using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamDinnerOD
{
    public partial class Choose : Form
    {
        /******************************************
            * 2025-01-06: 正式區資料庫與測試區有差別
            * 2025-01-06: 初版
            * 2025-01-13: 新增個人訪客
        ******************************************/
        // 正式區 = [db_owner].[BookDinnerGP]     測試區 = BookDinnerGP
        // 正式區 = [db_owner].[BookDinnerLog]    測試區 = BookDinnerLog

        public Choose()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //隱藏窗體
            this.Hide();

            // 開啟個人功能視窗
            using (Form2 form2 = new Form2())
            {
                form2.ShowDialog();  // 這會顯示 TeamVT 且阻止回到 Form1
            }
            this.Close(); // 關閉選擇窗體
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //隱藏窗體
            this.Hide();

            // 開啟個人功能視窗
            using (Form1 form1 = new Form1())
            {
                form1.ShowDialog();  // 這會顯示 TeamVT 且阻止回到 Form1
            }
            this.Close(); // 關閉選擇窗體
        }
    
    }
}
