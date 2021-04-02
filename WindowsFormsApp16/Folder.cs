using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApp16
{
    class Folder
    {
        private string myPath;//путь от виртуального диска до этой папки
        private string[] myFolders;//подкаталоги данного каталога
        private string[] myFiles;//файлы данного каталога
        private Folder[] myFold;//массив, хранящий объекты для каталогов текущего каталога
        private bool visible = true;//показывает, можно ли получить доступ к файлам каталога

        public Folder(string _myPath, bool _visible, int deep)
        {
            myPath = _myPath;
            visible = _visible;
            if (deep == 0) return;//deep является ограничителем для рекурсивного заполнения массива myFold
            if (visible == true)
            {
                myFolders = Directory.GetDirectories(myPath);
                myFiles = Directory.GetFiles(myPath);
                myFold = GetColl(deep);
            }
            else
            {
                myPath = _myPath;
                myFolders = new string[1];
                myFolders[0] = "";
                try { myFiles = Directory.GetFiles(myPath); }
                catch { myFiles = new string[1]; myFiles[0] = ""; }
                finally { myFiles = new string[1]; myFiles[0] = ""; }
            }
        }//конструктор создания объектов

        private Folder[] GetColl(int deep)
        {

            int cnt = -1;
            Folder[] toReturn = new Folder[myFolders.Length];
            foreach (string str in myFolders)
            {
                bool check = true;
                string s = str.Substring(myPath.Length);
                try
                {
                    s = s.Substring(0, 2);
                }
                catch{
                    s = "";
                }
                if ( s != "" )
                if (s[0] == '$' || s == "\\$") check = false;//проверка на скрытые системные папки
                try
                {
                    Directory.GetDirectories(str);
                    Directory.GetFiles(str);
                }
                catch
                {
                    check = false;
                }
                toReturn[++cnt] = new Folder(str, check,deep - 1);
            }
            return toReturn;
        }//воозвращает коллекцию объектов для заполнения массива myFold

        public string GetDir()
        {
            return myPath;
        }//возвращает myPath

        private bool GetVisibility()
        {
            return visible;
        }//возвращает visibility

        private int min(int x, int y)
        {
            return x > y ? y : x;
        }//метод, возвращающий минимальное значение из двух

        public string[] ShowInner(string _Dir)//возвращает строку, содержащую все подкаталоги и файлы текущей директории
        {
            string[] ToReturn;
            int cnt = -1;
            if (_Dir == myPath)
            {
                ToReturn = new string[myFiles.Length + myFolders.Length];
                foreach (string str in myFiles)
                    if (str.Substring(myPath.Length)[0] == '\\')
                        ToReturn[++cnt] = str.Substring(myPath.Length + 1);
                        else ToReturn[++cnt] = str.Substring(myPath.Length);
                for (int i = 0; i < myFold.Length; i++)
                    if (myFold[i].GetVisibility() == true)
                        if ( myFolders[i].Substring(myPath.Length)[0] == '\\')
                        ToReturn[++cnt] = myFolders[i].Substring(myPath.Length + 1);
                        else ToReturn[++cnt] = myFolders[i].Substring(myPath.Length);
                Array.Resize(ref ToReturn, cnt + 1);
                return ToReturn;
            }
            cnt = 0;
            foreach (string str in myFolders)
            {
                if (_Dir.Substring(0, min(str.Length, _Dir.Length)) == str) break;
                cnt++;
            }
            if ( cnt == myFold.Length )
            {
                cnt = -1;
                ToReturn = new string[myFiles.Length + myFolders.Length];
                foreach (string str in myFiles)
                    if (str.Substring(myPath.Length)[0] == '\\')
                        ToReturn[++cnt] = str.Substring(myPath.Length + 1);
                    else ToReturn[++cnt] = str.Substring(myPath.Length);
                for (int i = 0; i < myFold.Length; i++)
                    if (myFold[i].GetVisibility() == true)
                        if (myFolders[i].Substring(myPath.Length)[0] == '\\')
                            ToReturn[++cnt] = myFolders[i].Substring(myPath.Length + 1);
                        else ToReturn[++cnt] = myFolders[i].Substring(myPath.Length);
                Array.Resize(ref ToReturn, cnt + 1);
                return ToReturn;
            }
            myFold[cnt] = new Folder(myFolders[cnt], true, 1);
            return myFold[cnt].ShowInner(_Dir);
        }

        public string[] GetFiles()//возвращает коллекцию myFiles
        {
            return myFiles;
        }

        public Folder[] GetFolders()//возвращает коллекцию myFolders
        {
            return myFold;
        }

        public void Update( string _Dir, string newFile )//вызывает обновление директории, в которую скопировали файл
        {
            if ( _Dir == myPath )
            {
                string[] newFiles = new string[myFiles.Length + 1];
                for (int i = 0; i < myFiles.Length; i++)
                    newFiles[i] = myFiles[i];
                newFiles[myFiles.Length] = newFile;
                myFiles = newFiles;
            } else
            {
                foreach (Folder dr in myFold)
                    if (_Dir.Substring(0, min(dr.GetDir().Length, _Dir.Length)) == dr.GetDir())
                        dr.Update(_Dir, newFile); 
            }
        }

        public void NewName( string _Dir )//обновляет директорию, в которой было изменено название файла
        {
            if (_Dir == myPath)
                myFiles = Directory.GetFiles(myPath);
            else
                foreach (Folder dr in myFold)
                    if (dr.GetDir() == _Dir.Substring(0, min(_Dir.Length, dr.GetDir().Length)))
                        dr.NewName(_Dir);
        }
    }
}