using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace u0624011_SunshineFruit_Pos_System
{
    public partial class Form1 : Form
    {

        const String countUnit = "顆/份";
        List<Fruit> fruits = new List<Fruit>();
        List<Fruit> haveFruits = new List<Fruit>();
        Button [] ImgArea = new Button[19];
        String focusID = ""; //儲存被選中的項目ID
        int needMoney = 0;
        int getMoney = 0;
        Boolean mode = true; //是否正常使用

        public Form1()
        {
            InitializeComponent();

            //把集合用到陣列裡
            int setImg = 0;
            foreach (Control btn in tableLayoutPanel1.Controls) {
                if (btn is Button) {
                    ImgArea[setImg] = (Button)btn;
                    setImg++;
                }
            }
    }

        private void Form1_Load(object sender, EventArgs e)
        {
            for(int i = 0; i < ImgArea.Length; i++) { //設圖片
                ImgArea[i].ImageIndex = i;
            }

            FileInfo f = new FileInfo("fruits.txt");  //讀取設定值
            if (f.Exists) {
                StreamReader sr = f.OpenText();
                String str = "";
                while ((str = sr.ReadLine()) != null) {
                    String[] tmp = str.Split(',');
                    Fruit fruit = new Fruit(tmp[0], tmp[1]); //ID和名字
                    for (int i = 2; i < tmp.Length; i++) {
                        if (tmp[i].Contains("-")) { //存在單位
                            String[] tmp2 = tmp[i].Split('-');
                            fruit.WeightPrice = double.Parse(tmp2[0]);
                            if (tmp2[1] == "tg") {
                                fruit.WeightUnit = "臺斤";
                            } else if (tmp2[1] == "kg") {
                                fruit.WeightUnit = "公斤";
                            } else {
                                fruit.WeightUnit = tmp2[1];
                            }
                            
                        } else {
                            fruit.Price = double.Parse(tmp[i]);
                        }
                    }
                    fruits.Add(fruit);//新增物件 單一水果
                }
                sr.Close();
            }

        }


        private void FruitBtnClick(object sender, EventArgs e)
        {
            
            Button clickImg = (Button)sender;
            String listID,getID = "";
            int id= 0;

            listID = imageList1.Images.Keys[clickImg.ImageIndex]; //品名
            String[] splitW = listID.Split('.');
            getID = splitW[0];

            for(int i = 0; i < fruits.Count; i++) {
                if (fruits[i].ID == getID) {
                    id = i;
                    break;
                }
            }

                String showW = fruits[id].Name;

                if (fruits[id].WeightPrice != 0.0) {
                    showW += "　" + Convert.ToString(fruits[id].WeightPrice) + "/" + fruits[id].WeightUnit;
                }
                if (fruits[id].Price != 0.0) {
                    showW += "," + Convert.ToString(fruits[id].Price) + "/" + countUnit;
                }
                showUnit.Text = showW;


            if (mode) {
                if (!haveFruits.Contains(fruits[id])) { //判斷重複
                haveFruits.Add(fruits[id]);
                ListViewItem lvi = new ListViewItem(fruits[id].ID);
                lvi.SubItems.Add(fruits[id].Name);
                if (fruits[id].WeightPrice != 0.0) { //如果有小數點
                    lvi.SubItems.Add(Convert.ToString(fruits[id].WeightPrice));
                    lvi.SubItems.Add(fruits[id].WeightUnit);
                    lvi.SubItems.Add("1");
                    lvi.SubItems.Add(Convert.ToString(fruits[id].WeightPrice));
                    lab_Unit.Text = fruits[id].WeightUnit;
                } else {
                    lvi.SubItems.Add(Convert.ToString(fruits[id].Price));
                    lvi.SubItems.Add(countUnit);
                    lvi.SubItems.Add("1");
                    lvi.SubItems.Add(Convert.ToString(fruits[id].Price));
                    lab_Unit.Text = countUnit;
                }
                listView1.Items.Insert(0, lvi);
                txtTotal.Text = sumTotal(); //小計加總
            } else {
                if (fruits[id].Price != 0.0) { //轉換數量模式
                    Click2(fruits[id]);
                }
            }

   
            focusID = fruits[id].ID;
        }
        }

        private void Click2(Fruit fruit)
        {
            int findF = find_IdinListview(fruit.ID);

            //檢查模式是否已被更改
            if (double.Parse(listView1.Items[findF].SubItems[2].Text) == fruit.Price) {
                updateQ(fruit.ID, 1);
            } else {
                listView1.Items[findF].SubItems[2].Text = Convert.ToString(fruit.Price);
                listView1.Items[findF].SubItems[3].Text = countUnit;
                lab_Unit.Text = countUnit;
                updateQ(fruit.ID, 0); 
            }

        }

        private int find_IdinListview(String id)
        {
            int findF = -1;
            for (int i = 0; i < listView1.Items.Count; i++) {
                if (listView1.Items[i].SubItems[0].Text == id) {
                    findF = i;
                    break;
                }
            }
            return findF;
        }

        private void updateQ(String id,double add)
        {
            int findF = find_IdinListview(id);

            double Have = double.Parse(listView1.Items[findF].SubItems[4].Text);
            double Price = double.Parse(listView1.Items[findF].SubItems[2].Text);
            Have += add;
            listView1.Items[findF].SubItems[4].Text = Convert.ToString(Have);

            listView1.Items[findF].SubItems[5].Text = Convert.ToString(Price*Have);

            txtTotal.Text = sumTotal(); //小計加總
        }

        private String sumTotal()
        {
            double totalOkind = 0;
           for(int i = 0; i < listView1.Items.Count; i++) {
                double pp = double.Parse(listView1.Items[i].SubItems[2].Text);
                double qq = double.Parse(listView1.Items[i].SubItems[4].Text);
                totalOkind += (pp * qq);
            }
           String output = Convert.ToString((int)totalOkind)+" ";
            return output;
        }

        class Fruit {   //水果物件

            public string ID { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public double WeightPrice { get; set; }
            public String WeightUnit { get; set; }

            public Fruit(String id,String name)
            {
                ID = id;
                Name = name;
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (!mode) {
                mode = true;
            }
            listView1.Items.Clear();
            haveFruits.Clear();
            txtTotal.Text = "0";

            input_Box.Text = "";
            button34.Enabled = true;
            //接save 清空
            for (int i = 0; i < ImgArea.Length; i++) { //設圖片
                ImgArea[i].Enabled = true;
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
                input_Box.Text += "0";
        }

        private void button35_Click(object sender, EventArgs e)
        {
            if (input_Box.Text != "" & !input_Box.Text.Contains(".")) {
                input_Box.Text += ".";
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            if (input_Box.Text != "") {
                input_Box.Text = input_Box.Text.Substring(0, input_Box.Text.Length - 1);
            }
            }

        private void button29_Click(object sender, EventArgs e)
        {
            input_Box.Text += "1";
        }

        private void button30_Click(object sender, EventArgs e)
        {
            input_Box.Text += "2";
        }

        private void button31_Click(object sender, EventArgs e)
        {
            input_Box.Text += "3";
        }

        private void button25_Click(object sender, EventArgs e)
        {
            input_Box.Text += "4";
        }

        private void button26_Click(object sender, EventArgs e)
        {
            input_Box.Text += "5";
        }

        private void button27_Click(object sender, EventArgs e)
        {
            input_Box.Text += "6";
        }

        private void button21_Click(object sender, EventArgs e)
        {
            input_Box.Text += "7";
        }

        private void button22_Click(object sender, EventArgs e)
        {
            input_Box.Text += "8";
        }

        private void button23_Click(object sender, EventArgs e)
        {
            input_Box.Text += "9";
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (focusID != ""&input_Box.Text!="") {
                int findF = find_IdinListview(focusID);
                double Have = double.Parse(input_Box.Text);
                double Price = double.Parse(listView1.Items[findF].SubItems[2].Text);
                if (focusID != "0") {
                    listView1.Items[findF].SubItems[4].Text = Convert.ToString(Have);
                    listView1.Items[findF].SubItems[5].Text = Convert.ToString(Price * Have);
                } else {
                    listView1.Items[findF].SubItems[2].Text = Convert.ToString(Have);
                    listView1.Items[findF].SubItems[5].Text = Convert.ToString(1 * Have);
                }
                input_Box.Text = "";
                updateQ(focusID, 0);
            } else {
                MessageBox.Show("請選擇水果項目","提示");
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            if (mode) {
            MessageBox.Show("錢櫃已開啟!","警告"); }
        }

        private void saveData()
        {
            check.Text = "結帳";
            check.BackColor = Color.PaleGoldenrod;

            String datetime = DateTime.Now.ToString("yyyy-MM-dd");
            datetime += "　"+ DateTime.Now.ToShortTimeString();

            FileInfo f = new FileInfo("Records.txt");
            StreamWriter sw = f.AppendText();

            sw.WriteLine(datetime);
            for (int i = 0; i < listView1.Items.Count; i++) {
                string str = Convert.ToString(i)+"- ";
                str += listView1.Items[i].SubItems[1].Text;
                str += ",數量:";
                str += listView1.Items[i].SubItems[4].Text;
                str += ",小計:";
                str += listView1.Items[i].SubItems[5].Text;
                sw.WriteLine(str);
            }

            sw.WriteLine("總計:"+Convert.ToString(needMoney));
            sw.WriteLine("實收:" + Convert.ToString(getMoney));
            sw.WriteLine("找零:" + Convert.ToString(getMoney-needMoney));

            sw.WriteLine("--------------------------");
            sw.Flush();
            sw.Close();
        }

        private void button28_Click(object sender, EventArgs e)
        {

            if (check.Text == "結帳") {
                for (int i = 0; i < ImgArea.Length; i++) { //設圖片
                    ImgArea[i].Enabled = false;
                }
                button37.Enabled =  button38.Enabled = button39.Enabled = button40.Enabled = true;

                input_Box.Text = "";
                button34.Enabled = false;
                needMoney = int.Parse(txtTotal.Text);
                lab_Unit.Text = "新台幣";
                check.Text = "完成訂單";
                check.BackColor = Color.Pink;
            } else {

                if (input_Box.Text != "") {
                    getMoney = int.Parse(input_Box.Text);
                    button37.Enabled = button38.Enabled = button39.Enabled = button40.Enabled = false;
                    if (getMoney - needMoney > 0) {
                        input_Box.Text = "找零 " + Convert.ToString(getMoney - needMoney);
                        saveData();
                    } else if (getMoney - needMoney == 0) {
                        input_Box.Text = "免找零";
                        saveData();
                    } else {
                        MessageBox.Show("實收金額不足!","錯誤");
                        input_Box.Text = "";
                    }
                }
            }
        }

        private void otherOff(double offP,String name) {
            
                double oldNeed, newNeed = 0.0;
                double tmpNum = double.Parse(txtTotal.Text);
                oldNeed = tmpNum;
                tmpNum = tmpNum * offP;
                newNeed = tmpNum;
                txtTotal.Text = Convert.ToString(newNeed);

                ListViewItem lvi = new ListViewItem("~");
                lvi.SubItems.Add(name);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add(Convert.ToString(newNeed - oldNeed));
                listView1.Items.Add(lvi);
  
        }

        private void button37_Click(object sender, EventArgs e)
        {
            otherOff(0.95, "95折優惠");
        }

        private void button38_Click(object sender, EventArgs e)
        {
            otherOff(0.9, "9折優惠");
        }

        private void button40_Click(object sender, EventArgs e)
        {
            otherOff(0.8, "8折優惠");
        }

        private void button39_Click(object sender, EventArgs e)
        {
            otherOff(0.5, "5折優惠");
        }

        private void button32_Click(object sender, EventArgs e)
        {
            if (mode) {
            if (focusID != "") {
                int findF = find_IdinListview(focusID);
                listView1.Items[findF].Remove();
                txtTotal.Text = sumTotal(); //小計加總
                for(int i = 0; i < haveFruits.Count;i++) {
                    if(haveFruits[i].ID == focusID) {
                        haveFruits.RemoveAt(i);
                        break;
                    }
                }
                focusID = "";
            } else {
                if (haveFruits.Count > 0) {
                    int findF = find_IdinListview(haveFruits[haveFruits.Count - 1].ID);
                    haveFruits.RemoveAt(haveFruits.Count - 1);
                    listView1.Items[findF].Remove();
                    txtTotal.Text = sumTotal(); //小計加總
                }
            }
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            Process.Start(@"Records.txt");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            mode = false;
        }
    }
}
