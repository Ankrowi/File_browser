using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp16
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MultiDrive mdrive;
        bool start = true;
        bool canCopy = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Text = "↑";
            button2.Text = "Копировать";
            button3.Text = "Вставить";
            button4.Text = "Переименовать";
            textBox2.Enabled = false;
            textBox2.Visible = false;
            button5.Visible = false;
            button5.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            listBox1.Font = new System.Drawing.Font("Times New Roman", 10);
            mdrive = new MultiDrive();
        }//настройка первоначальных параметров при запуске программы
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (canCopy) button3.Visible = button3.Enabled = true;
            button2.Visible = button2.Enabled = false;
            button4.Visible = button4.Enabled = false;
            listBox1.Items.Clear();//первоначально требуется удалить все имеющиеся в коллекции listBox1 поля
            listBox1.Items.AddRange(mdrive.GoBack());//добавляем новые отображаемые поля
        }//метод, возвращающий предыдущую директорию(отвечает за кнопку стрелочки вверх)

        private void button2_Click(object sender, EventArgs e)
        {
            canCopy = true;
            string SelectedText = (string)listBox1.SelectedItem;
            mdrive.Remember(SelectedText);
        }//метод, запоминающий путь копируемого файла(обработчик кнопки Копировать)

        private void button3_Click(object sender, EventArgs e)
        {
            mdrive.Paste();
            listBox1.Items.Clear();
            listBox1.Items.AddRange(mdrive.ShowCurrent());
        }//метод копирующий файл в указанную директорию(обработчик кнопки Вставить)

        private void button4_Click(object sender, EventArgs e)
        {
            string strBefore = (string)listBox1.SelectedItem;
            listBox1.Visible = false;
            listBox1.Enabled = false;
            textBox2.Enabled = true;
            textBox2.Visible = true;
            textBox2.Text = "Введите новое название(без расширения)";
            button5.Text = "OK";
            button5.Visible = true;
            button5.Enabled = true;
            mdrive.RemName(strBefore);
        }//метод, вызывающий поле, в которое вводится новое название файла

        private void button5_Click(object sender, EventArgs e)//метод возвращающий обозреватель каталогов и переименовывающий нужный файл(обработчик кнопки OK)
        {
            mdrive.ReName(textBox2.Text);
            textBox2.Visible = false;
            textBox2.Enabled = false;
            listBox1.Visible = true;
            listBox1.Enabled = true;
            listBox1.Items.Clear();
            listBox1.Items.AddRange(mdrive.ShowCurrent());
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string SelectedText = "";
            try
            {
                SelectedText = (string)listBox1.SelectedItem;
            }
            catch
            {
                return;
            }
            if (SelectedText == null) return; 
            listBox1.Items.Clear();
            listBox1.Items.AddRange(mdrive.ShowInner(SelectedText));
            button2.Enabled = button2.Visible = false;
            button4.Enabled = button4.Visible = false;
        }//метод, возвращающий содержмиое требуемого каталога(происходит при двойном клике на панели)

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (start == true)
            {
                button1.Visible = button1.Enabled = true;
                listBox1.Items.Clear();
                listBox1.Items.AddRange(mdrive.ShowInner(""));
                start = false;
            }
            else
            {
                if (canCopy == true) button3.Visible = button3.Enabled = true;
                button2.Visible = button2.Enabled = true;
                button4.Visible = button4.Enabled = true;
                start = false;
            }
        }//метод, выводящий подключенные логические диски, после нажатия на кнопку мыши, также отвечает за отображение кнопок на форме

        private void textBox2_MouseClick(object sender, MouseEventArgs e)//метод, очищающий поле при клике на него(для удобного ввода нового имени файла)
        {
            textBox2.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)//обработчик кнопки Завершить, выполняющий завершение работы программы
        {
            Close();
        }
    }
}
