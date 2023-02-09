using System;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Diagnostics;
//using System.Runtime.Intrinsics.Arm;

namespace passmanager
{

    public partial class MainForm : Form
    {
        //データの保存ファイル名
        private string _AccountInfoFile = "AccountInfo.xml";

        //多重起動禁止
        public CreatePassWord CPW = null;
        public AES DW = null;

        //画面に配置するコントロールの登録と初期化
        public MainForm()
        {
            //画面に置かれたコントロール類に初期値を設定し、画面を描画するメソッド
            InitializeComponent();

            //プログラムをディスプレイの真ん中に表示
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // 画面表示時の初期化処理
        private void MainForm_Load(object sender, EventArgs e)
        {
            load();
        }

        //更新ボタン
        private void reload_Click(object sender, EventArgs e)
        {
            load();
        }

        //フォームの読み込み
        private void load()
        {
            //ウィンドウのサイズを固定にする
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //IDパスワード一覧に表示するデータの作成
            DataTable dt = new DataTable();
            dt.Columns.Add("カテゴリ");
            dt.Columns.Add("アプリ/サイト名");
            dt.Columns.Add("ID");
            dt.Columns.Add("メールアドレス");
            dt.Columns.Add("パスワード");
            dt.Columns.Add("URL");
            dt.Columns.Add("備考");
            //テーブル名を設定
            dt.TableName = "AccountInfo";


            //既に保存済みの/IDパスワードファイルがあれば、読み込む
            if (File.Exists(_AccountInfoFile))
            {
                dt.ReadXml(_AccountInfoFile);
            }

            ///IDパスワード一覧に表示
            PassWordGrid.DataSource = dt;

            //IDパスワード一覧の初期設定（UI）
            PassWordGrid.SelectionMode = DataGridViewSelectionMode.CellSelect; //行選択モードの設定
            PassWordGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;//列を左右一杯に広げる
            PassWordGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.Linen;//1行置きの背景色を設定
            PassWordGrid.ReadOnly = true;//書き込み禁止の設定
            PassWordGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;//コピーした値にヘッダを含まない
            PassWordGrid.AllowUserToAddRows = false;//ユーザーの行追加を禁止にする
            RefreshDropDownList();//カテゴリドロップダウンの再作成
        }

        //暗号化ボタン
        private void encbutton_Click(object sender, EventArgs e)
        {
            if (this.DW == null || this.DW.IsDisposed)
            {
                this.DW = new AES();
                DW.Show();
            }
        }
        
        public void DC()
        {
            load();
        }

        //カテゴリドロップダウンリスト表示時の処理
        private void Category_DropDown(object sender, EventArgs e)
        {
            //カテゴリドロップダウンリストの中身が０なら、作り直す
            if (Category.Items.Count == 0)
            {
                RefreshDropDownList();
            }
        }

        // カテゴリドロップダウンの再作成
        private void RefreshDropDownList()
        {
            Category.Items.Clear();

            Category.Items.Add("");

            for (int i = 0; i < PassWordGrid.Rows.Count; i++)
            {
                var category = PassWordGrid["カテゴリ", i].Value.ToString();

                if (!Category.Items.Contains(category))
                {
                    Category.Items.Add(category);
                }
            }

        }

        // カテゴリドロップダウンリスト表示時の処理
        private void Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dt = (DataTable)PassWordGrid.DataSource;

            if (Category.Text == "")
            {
                dt.DefaultView.RowFilter = "";
            }
            else
            {
                dt.DefaultView.RowFilter = "カテゴリ = '" + Category.Text + "'";
            }
        }

        //IDをクリップボードに取得する処理（ID取得）
        private void IDClip_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("出力");
            //選択行に登録されているIDを取得
            var row = PassWordGrid.CurrentRow;
            var id = row.Cells["ID"].Value.ToString();

            if (id != "")
            {
                //クリップボードへコピー
                Clipboard.SetText(id);
            }
        }

        //メールアドレスをクリップボードに取得する処理（メールアドレス取得）
        private void MailClip_Click(object sender, EventArgs e)
        {
            //選択行に登録されているIDを取得
            var row = PassWordGrid.CurrentRow;
            var mail = row.Cells["メールアドレス"].Value.ToString();

            if (mail != "")
            {
                //クリップボードへコピー
                Clipboard.SetText(mail);
            }
        }

        //パスワードをクリップボードに取得する処理（パスワード取得）
        private void PassClip_Click(object sender, EventArgs e)
        {
            //選択行に登録されているパスワードを取得
            var row = PassWordGrid.CurrentRow;
            var pwd = row.Cells["パスワード"].Value.ToString();

            if (pwd != "")
            {
                //クリップボードへコピー
                Clipboard.SetText(pwd);
            }
        }

        //編集モード変更の処理（編集）
        private void EditMode_Click(object sender, EventArgs e)
        {
            //ReadOnlyプロパティの状態を反転
            PassWordGrid.ReadOnly = !PassWordGrid.ReadOnly;

            //IDパスワード一覧の書き込み禁止モードを有効にする
            if (PassWordGrid.ReadOnly == true)
            {
                //編集ボタンのテキストと背景色を設定（最初の状態に戻す）
                EditMode.Text = "編集";
                EditMode.BackColor = SystemColors.Control;
                DeleteButton.BackColor = SystemColors.Control;

                //ユーザーによる行追加を禁止にする
                PassWordGrid.AllowUserToAddRows = false;

                //カテゴリドロップダウンリストを作り直す
                RefreshDropDownList();
            }
            else
            {
                //編集ボタンのテキストと背景色（黄色）を編集中に設定
                EditMode.Text = "編集中";
                EditMode.BackColor = Color.Yellow;
                //同時に削除ボタンの色も変更
                DeleteButton.BackColor = Color.Yellow;

                //ユーザーによる行追加を有効にする
                PassWordGrid.AllowUserToAddRows = true;
            }
        }

        //XMLに保存する処理（保存）
        private void SaveButton_Click(object sender, EventArgs e)
        {
            //IDパスワード一覧をファイルに保存
            DataTable dt = (DataTable)PassWordGrid.DataSource;
            dt.WriteXml(_AccountInfoFile);

            //メッセージの表示
            MessageBox.Show("保存しました");
        }

        //URLをクリップボードに取得する処理（URL取得）
        private void URLButton_Click(object sender, EventArgs e)
        {
            //選択行に登録されているパスワードを取得
            var row = PassWordGrid.CurrentRow;
            var url = row.Cells["URL"].Value.ToString();

            if (url != "")
            {
                //クリップボードへコピー
                Clipboard.SetText(url);
            }
        }

        //キー入力の処理 
        private void PassWordGrid_KeyDown(object sender, KeyEventArgs e)
        {
            //書き込みの可否
            if (PassWordGrid.ReadOnly)
            {
                return;
            }

            //最下段（空白行）の時
            if (PassWordGrid.CurrentRow.IsNewRow == true)
            {
                return;
            }

            //DELキーが押された時
            if (e.KeyCode == Keys.Delete)
            {
                //選択行を削除
                PassWordGrid.Rows.Remove(PassWordGrid.CurrentRow);
            }
        }

        //削除ボタンの処理
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            //書き込みの可否
            if (PassWordGrid.ReadOnly)
            {
                return;
            }
            //最下段（空白行）の時
            else if (PassWordGrid.CurrentRow.IsNewRow == true)
            {
                return;
            }
            //それ以外
            else
            {
                //選択行を削除
                PassWordGrid.Rows.Remove(PassWordGrid.CurrentRow);
            }
        }

        //パスワード生成ウィンドウの表示
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.CPW == null || this.CPW.IsDisposed)
            {
                // nullまたは破棄されていたら
                this.CPW = new CreatePassWord();
                CPW.Show();
                //CreatePassWord createPassWord = new CreatePassWord();
                //createPassWord.Show();
            }

        }

        //デフォルトのブラウザーでURLのページを開く処理
        private void Browser_Click(object sender, EventArgs e)
        {
            var row = PassWordGrid.CurrentRow;
            var url = row.Cells["URL"].Value.ToString();
            //URLのセルが空白だった場合
            if (row.Cells["URL"].Value.ToString() == null || row.Cells["URL"].Value.ToString() == "")
            {
                return;
            }
            else
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo(url);
                startInfo.UseShellExecute = true;
                System.Diagnostics.Process.Start(startInfo);
            }
        }
    }
}
