using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApp16
{
    class MultiDrive
    {
        Folder[] Folders;// коллекция логических дисков
        private int level = 0;//показывает уровень погружения
        private string CurrentLocation = "";//положение пользователя
        private string nameToCopy = "";//хранит полный путь копируемого файла
        private string myPath = "";
        private string strToRemember;//хранит первоначальный путь переименовываемого файла

        public MultiDrive()//конструктор
        {
            string[] driv = Environment.GetLogicalDrives();//получение списка логических дисков
            Folders = new Folder[driv.Length];
            int cnt = -1;
            foreach( string dr in driv )
            {
                Folders[++cnt] = new Folder(dr, true, 1);//создание для каждого логического диска своего объекта, который будет содержать список файлов и папок, в нём хранящихся
            }
        }

        private bool isFile(string Add)
        {
            bool ToReturn = false;
            int dots = 0;
            foreach( char c in Add )
                if ( c == '.' ) dots++;
            if (dots == 1) ToReturn = true;
            return ToReturn;
        }

        public void UpdateCurrent(string Add)
        {
            //необходимость знания "уровня" погружения связана с отображением пути файла в системе, в зависимости от её уровня
            if (level == 0)
            {
                CurrentLocation = Add + "\\";//если level = 0, то пользователь выбирает требующийся логический диск
            }
            else if (level == 1)
            {
                CurrentLocation += Add;//если level = 1, то пользователь находится в требуемой логическом диске и выбирает нужную директорию в нём
            }
            else
            {
                CurrentLocation += "\\" + Add;
            }//в зависимости от текущего уровня нахождения добавляет к текущему пути новую директорию
        }//метод, добавляющий к текущему положению пользователя название новой директории

        public string[] ShowInner(string Add)//возвращает содержимое новой директории
        {
            string[] strToReturn = null;//возвращаемый массив, который потом будет отображён в коллекции поля listBox1
            if (isFile(Add) == true) return ShowCurrent();
            if (Add == "")//происходит при первоначальном отображении списка логических дисков
            {
                strToReturn = new string[Folders.Length];
                int cnt = -1;
                foreach (Folder dr in Folders)
                {
                    strToReturn[++cnt] = dr.GetDir().Substring(0,2);
                }
            }
            else UpdateCurrent(Add);

            if (CurrentLocation != "")
            {
                foreach (Folder dr in Folders)
                {
                    if (CurrentLocation.Substring(0, 2) == dr.GetDir().Substring(0, 2))//проверка, в какой из логических дисков нужно отправить рекурсивный вызов для получения нужной пользователю коллекции
                        strToReturn = dr.ShowInner(CurrentLocation);//вызывает аналогичный метод у нужного логического диска
                }
                level++;
            }
            return strToReturn;
        }

        public string[] GoBack()//возвращает содержимое предыдущей директории
        {
            string[] strToReturn = null;
            int otr = 0;
            for (int i = CurrentLocation.Length - 1; i > 0; i--)//убирает последнюю директорию из текущего положения
                if (CurrentLocation[i] == '\\') break;
                else otr++;
            if (level > 2)//в зависимости от текущего показателя level возвращает содержимое нужной директории
            {
                CurrentLocation = CurrentLocation.Substring(0, CurrentLocation.Length - otr - 1);
                foreach( Folder dr in Folders )
                {
                    if ( CurrentLocation.Substring(0, 2) == dr.GetDir().Substring(0, 2))
                    {
                        strToReturn = dr.ShowInner(CurrentLocation);
                        level--;
                    }
                }
            }
            else if ( level == 1 || level == 0 )
            {
                strToReturn = new string[Folders.Length];
                int cnt = -1;
                foreach (Folder driver in Folders)
                {
                    strToReturn[++cnt] = driver.GetDir().Substring(0, 2);
                }
                CurrentLocation = "";
                level = 0;
            }
            else if (level == 2)
            {
                CurrentLocation = CurrentLocation.Substring(0, CurrentLocation.Length - otr);
                foreach( Folder driver in Folders )
                {
                    if ( CurrentLocation.Substring(0, 2) == driver.GetDir().Substring(0, 2))
                    {
                        strToReturn = driver.ShowInner(CurrentLocation);
                        level--;
                    }
                }
            }
            return strToReturn;
        }

        public void Remember( string Add )//запоминает путь до файла, который нужно скопировать
        {
            string ToRemember = CurrentLocation;
            if (level == 1)//добавляет к текущей директории новый файл для копирования
            {
                ToRemember += Add;
            } else if ( level == 0 )
            {
                ToRemember = Add + "\\";
            } else
            {
                ToRemember += "\\" + Add;
            }
            nameToCopy = ToRemember;
        }

        private void Update(string _Dir, string newFile)//обновляет директорию после копирования, _Dir - путь  каталога, newFile - название файла(копируемого)
        {
            foreach( Folder dr in Folders )
                if ( dr.GetDir() == _Dir.Substring(0, dr.GetDir().Length) )
                    dr.Update(_Dir, newFile);
        }

        public void Paste()//вставляет копируемый файл в нужную директорию()
        {
            //узнаёт путь до искомого копируемого файла
            if (nameToCopy == "") return;
            int otr = 0;
            for (int i = nameToCopy.Length - 1; i > 0; i--)
                if (nameToCopy[i] == '\\') break;
                else otr++;

            try//копирует файл
            {
                File.Copy(nameToCopy, CurrentLocation + "\\" + nameToCopy.Substring(nameToCopy.Length - otr - 1));
            }
            catch
            {
                return;
            }
            //вызывает обновление необходимой директории
            string filename = CurrentLocation + nameToCopy.Substring(nameToCopy.Length - otr - 1);
            Update(CurrentLocation, filename);
        }

        public string[] ShowCurrent()//возвращает каталоги и файлы текущей директории
        {
            string[] strToReturn = null;
            if (CurrentLocation == "") return ShowInner("");
            foreach (Folder dr in Folders)
            {
                if (CurrentLocation.Substring(0, 2) == dr.GetDir().Substring(0, 2))
                    strToReturn = dr.ShowInner(CurrentLocation);
            }
            return strToReturn;
        }

        public void RemName(string Add)//запоминает имя переименовываемого файла
        {
            string strToName = CurrentLocation;
            if (level == 0)
            {
                strToName = Add + "\\";
            }
            else if (level == 1)
            {
                strToName += Add;
            }
            else
            {
                strToName += "\\" + Add;
            }
            strToRemember = strToName;
        }

        private void NewName()//обновляет каталог, в котором был переименован файл
        {
            if (CurrentLocation == "") return;
            foreach(Folder dr in Folders)
            {
                if (dr.GetDir().Substring(0, 2) == CurrentLocation.Substring(0, 2))
                    dr.NewName(CurrentLocation);//вызывает обновление в необходимом диске
            }
        }

        public void ReName(string newName)//переименовывает необходимый файл
        {
            int otr = 0;
            for (int i = 0; i < strToRemember.Length; i++)
                if (strToRemember[i] == '.') otr = i;
            string extention = strToRemember.Substring(otr);//выделяет расширение файла
            try 
            { 
                File.Move(strToRemember, CurrentLocation + "\\" + newName + extention);//перемещает файл в эту же директорию, но с другим именем
            } catch
            {

            }
            NewName();//обновляет список файлов в необходимой директории
        }
    }
}