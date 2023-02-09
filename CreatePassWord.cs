using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace passmanager
{
    public partial class CreatePassWord : Form
    {
        public CreatePassWord()
        {
            //画面に置かれたコントロール類に初期値を設定し、画面を描画するメソッド
            InitializeComponent();
            //プログラムをディスプレイの真ん中に表示
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void CreatePassWord_Load(object sender, EventArgs e)
        {
            //ウィンドウのサイズを固定にする
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            var eigo_komoji = "abcdefghijklmnopqrstuvwxyz";
            var eigo_Omoji = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var suuji = "0123456789";
            var kigou = "!#$%";
            var result = "";
            var rnd = new Random();
            String syuturyoku = "";
            var n = Convert.ToInt32(textBox1.Text);

            //checkBox1　英字大文字
            //checkBox2　英字小文字
            //checkBox3　数字
            //checkBox4　記号

            if (checkBox1.Checked == true)result = result + eigo_Omoji;
            if (checkBox2.Checked == true)result = result + eigo_komoji;
            if (checkBox3.Checked == true)result = result + suuji;
            if (checkBox4.Checked == true)result = result + kigou;

            //いずれかにチェックが入っていた場合
            if (checkBox1.Checked == true || checkBox2.Checked == true || 
                checkBox3.Checked == true || checkBox4.Checked == true) 
            {
                for (int i = 0; i < n; i++)
                {
                    syuturyoku += result[rnd.Next(result.Length)];
                }
                Clipboard.SetText(syuturyoku);
            }
            //error処理
            else if (checkBox1.Checked == false && checkBox2.Checked == false && 
                checkBox3.Checked == false && checkBox4.Checked == false)
            {
                //メッセージの表示
                MessageBox.Show("いずれかにチェックを入れてください");
            }
            textBox2.Text = syuturyoku;
        }
    }
}
