using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TeamDinnerOD
{
    public partial class TeamVT : Form
    {

        public TeamVT()
        {
            InitializeComponent();
        }

        // 當表單加載時，設置所有 TextBox 的預設提示文字
        private void TeamVT_Load(object sender, EventArgs e)
        {
            //下拉選單(餐別)
            DinnerType.Items.Add("中餐"); DinnerType.Items.Add("晚餐");//增加下拉選單的項目
            DinnerType.DropDownStyle = ComboBoxStyle.DropDownList;// 只允許選擇項目，不能輸入其他內容
            DinnerType.SelectedItem = "中餐";// 設置選中項目為 "中餐"
            //下拉選單(部門)
            comboBox1.Items.Add("3M/C"); comboBox1.Items.Add("5M/C"); comboBox1.Items.Add("6M/C");
            comboBox1.Items.Add("7M/C"); comboBox1.Items.Add("A區"); comboBox1.Items.Add("B區");
            comboBox1.Items.Add("C區"); comboBox1.Items.Add("D區"); comboBox1.Items.Add("ISO中心");
            comboBox1.Items.Add("PM3B"); comboBox1.Items.Add("RD研發"); comboBox1.Items.Add("土建監工");
            comboBox1.Items.Add("文件中心"); comboBox1.Items.Add("水環保課"); comboBox1.Items.Add("出差人員");
            comboBox1.Items.Add("出差主管"); comboBox1.Items.Add("安管"); comboBox1.Items.Add("安管守衛");
            comboBox1.Items.Add("品管課"); comboBox1.Items.Add("倉檢課"); comboBox1.Items.Add("財會");
            comboBox1.Items.Add("配送交貨"); comboBox1.Items.Add("配送成品"); comboBox1.Items.Add("控源");
            comboBox1.Items.Add("排抄"); comboBox1.Items.Add("統購"); comboBox1.Items.Add("統購中心");
            comboBox1.Items.Add("減廢"); comboBox1.Items.Add("新進"); comboBox1.Items.Add("業務");
            comboBox1.Items.Add("業務客服"); comboBox1.Items.Add("資訊科技"); comboBox1.Items.Add("電儀電氣");
            comboBox1.Items.Add("電儀儀錶"); comboBox1.Items.Add("預保"); comboBox1.Items.Add("製程技術");
            comboBox1.Items.Add("製漿課"); comboBox1.Items.Add("廠務管理"); comboBox1.Items.Add("熱電");
            comboBox1.Items.Add("總務HR人"); comboBox1.Items.Add("總務HR事"); comboBox1.Items.Add("總務來賓");
            comboBox1.Items.Add("總務外籍"); comboBox1.Items.Add("總務餐飲"); comboBox1.Items.Add("總處資訊");
            comboBox1.Items.Add("購料"); comboBox1.Items.Add("擴建專案");
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            //先鎖定必要資訊填寫
            txtUserId.Enabled = false;
            txtName.Enabled = false;
            txtMember.Enabled = false;
            txtReason.Enabled = false;
            comboBox1.Enabled = false;
            DinnerType.Enabled = false;
            button1.Enabled = false;

            // 設置每個 TextBox 的提示文字並設置顏色為灰色
            foreach (Control control in this.Controls)
            {
                // 使用 as 來轉換為 TextBox
                TextBox textBox = control as TextBox;

                // 確保 control 轉換為 TextBox 且不為 null
                if (textBox != null)
                {
                    // 根據 TextBox 的名稱來設置 Tag 屬性
                    if (textBox.Name == "txtCardNo") textBox.Tag = "請輸入卡號";
                    else if (textBox.Name == "txtName") textBox.Tag = "請輸入被訪人";
                    else if (textBox.Name == "txtReason") textBox.Tag = "請輸入原因";
                    else if (textBox.Name == "txtUserId") textBox.Tag = "請輸入工號";
                    else if (textBox.Name == "txtMember") textBox.Tag = "請輸入人數";

                    // 設置 TextBox 顯示 Tag 作為提示文字
                    textBox.Text = textBox.Tag.ToString();  // 設置提示文字
                    textBox.ForeColor = System.Drawing.Color.Gray;  // 設置顏色為灰色

                    // 綁定 Enter 和 Leave 事件
                    textBox.Enter += new EventHandler(txtBox_Enter);
                    textBox.Leave += new EventHandler(txtBox_Leave);
                }
            }
        }

        // 當 TextBox 獲得焦點時，清空內容並設置顏色為黑色
        private void txtBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == textBox.Tag.ToString())  // 使用 Tag 存儲提示文字
            {
                textBox.Text = "";  // 清空內容
                textBox.ForeColor = System.Drawing.Color.Black;  // 設置顏色為黑色
            }
        }

        // 當 TextBox 失去焦點時，如果是空的就恢復提示文字
        private void txtBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();  // 恢復提示文字
                textBox.ForeColor = System.Drawing.Color.Gray;  // 設置顏色為灰色
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 檢查所有必填欄位是否都有填寫
            if (string.IsNullOrWhiteSpace(txtUserId.Text) ||
                (txtUserId.Tag != null && txtUserId.Text == txtUserId.Tag.ToString()))
            {
                MessageBox.Show("請填寫有效的工號", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string query = @"SELECT TOP(1) * FROM Employee
                     WHERE UserID = @userid AND (Flag IS NULL OR Flag = '')";

                using (SqlConnection connection = DatabaseHelper.GetConnection())  // 假設 DatabaseHelper 是你管理資料庫連線的類別
                {
                    try
                    {
                        // 開啟連線
                        connection.Open();

                        // 建立 SQL 命令並加入參數
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@userid", txtUserId.Text);


                            // 執行查詢並檢查是否有資料
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (!reader.HasRows)
                            {
                                // 資料庫中有符合條件的資料
                                MessageBox.Show("工號無效！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"資料庫操作錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(txtCardNo.Text) ||
                (txtCardNo.Tag != null && txtCardNo.Text == txtCardNo.Tag.ToString()))
            {
                MessageBox.Show("請填寫有效的卡號", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                (txtName.Tag != null && txtName.Text == txtName.Tag.ToString()))
            {
                MessageBox.Show("請填寫有效的姓名", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtReason.Text) ||
                (txtReason.Tag != null && txtReason.Text == txtReason.Tag.ToString()))
            {
                MessageBox.Show("請填寫說明", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇部門", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DinnerType.SelectedItem == null)
            {
                MessageBox.Show("請選擇餐點類型", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 從 txtMember.Text 取得人數
            int memberCount;
            if (!int.TryParse(txtMember.Text, out memberCount) || memberCount <= 1)
            {
                MessageBox.Show("人數必須大於1", "錯誤");
                return;  // 人數不合法時退出
            }

            DateTime startDate = dateTimePicker1.Value;  // 獲取開始日期
            DateTime endDate = dateTimePicker2.Value;    // 獲取結束日期

            // 檢查開始日期和結束日期是否選擇有效日期
            if (startDate.Date < DateTime.Now.Date || endDate.Date < DateTime.Now.Date)
            {
                MessageBox.Show("請選擇有效的開始日期!(日期不能小於今天)", "日期錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // 如果未選擇有效日期，則退出
            }

            //檢查開始日期是否小於結束日期
            if (startDate.Date > endDate.Date)
            {
                MessageBox.Show("開始日期晚於結束日期！", "日期錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // 如果日期不符合，直接return
            }

            long elapsedTicks = endDate.Date.Ticks - startDate.Date.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            int alldays = elapsedSpan.Days;  // 獲取開始日期到結束日期之間的天數，由於沒有檢查周末假日，所以+1才會新增到結束日期

            int Dtype = 0;

            // 確保 SelectedItem 不為 null
            if (DinnerType.SelectedItem != null)
            {
                if (DinnerType.SelectedItem.ToString() == "中餐")
                {
                    Dtype = 2;
                }
                else
                {
                    Dtype = 3;
                }
            }
            else
            {
                Dtype = 0; // 默認值
                MessageBox.Show("請選擇餐點類型");
                return;  // 如果沒有選擇餐點類型，則退出方法
            }

            DialogResult dr = MessageBox.Show("這是從開始日期至結束日期的\n           '團體訂餐'功能\n            請謹慎使用!!!", "特殊功能", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dr == DialogResult.OK)
            {
                using (SqlConnection sqc = DatabaseHelper.GetConnection())
                {
                    try
                    {
                        sqc.Open();  // 確保連接被打開

                        // 使用事務來確保資料的一致性
                        using (SqlTransaction transaction = sqc.BeginTransaction())
                        {
                            try
                            {
                                // 開始插入資料
                                List<string> addedDates = new List<string>();
                                for (int i = 0; i <= alldays; i++)
                                {
                                    string idtmp = Guid.NewGuid().ToString();  // 確保每次插入的 ID 是唯一的

                                    if (txtCardNo.Text == "0257964541")
                                    {
                                        // 當卡號為 "0257964541" 時執行以下方法
                                        InsertBookDinnerData(sqc, transaction, idtmp, startDate, memberCount, Dtype);
                                        DVInsertBaseBookData(sqc, transaction, idtmp, startDate, Dtype);
                                        DVInsertBookDinnerLogData(sqc, transaction, idtmp, startDate, Dtype);
                                    }
                                    else
                                    {
                                        // 當卡號不是 "0257964541" 時執行以下方法
                                        InsertBookDinnerData(sqc, transaction, idtmp, startDate, memberCount, Dtype);
                                        InsertBaseBookData(sqc, transaction, idtmp, startDate, Dtype);
                                        InsertBookDinnerLogData(sqc, transaction, idtmp, startDate, Dtype);
                                        InsertBookDinnerGPData(sqc, transaction, idtmp, memberCount);
                                    }

                                    addedDates.Add(startDate.ToString("yyyy-MM-dd"));

                                    // 增加一天
                                    startDate = startDate.AddDays(1);
                                }

                                // 提交事務
                                transaction.Commit();

                                // 顯示成功訊息
                                string dateRange = string.Join(", ", addedDates.Take(5)); // 顯示前5個日期
                                if (addedDates.Count > 5)
                                {
                                    dateRange += "..."; // 若超過5個日期，顯示省略號
                                }
                                MessageBox.Show($"卡號 : {txtCardNo.Text}\n受訪人: {txtName.Text}\n成功新增以下日期：\n{dateRange}", "新增成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                button1.Enabled = false;
                                this.Close();    // 關閉當前視窗
                            }
                            catch (Exception ex)
                            {
                                // 發生錯誤時回滾事務
                                transaction.Rollback();
                                MessageBox.Show($"資料庫操作出現錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"資料庫連接或操作錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        button1.Enabled = true;  // 確保完成後重新啟用按鈕
                    }
                }
            }
        }

        private void ClearFormData()
        {
            txtUserId.Text = string.Empty;
            txtCardNo.Text = string.Empty;
            txtName.Text = string.Empty;
            txtReason.Text = string.Empty;
            txtMember.Text = string.Empty;
            comboBox1.SelectedIndex = -1;
            DinnerType.SelectedItem = null;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }


        private void InsertBookDinnerData(SqlConnection sqc, SqlTransaction transaction, string idtmp, DateTime startDate, int memberCount, int Dtype)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO BookDinner (ID, DDate, BeVisit, Class, Department, Reason, CardNo, DType, Md1, Md2, Memo, dbs, Gp, Num2, Num) " +
                                            "VALUES (@Id, @Date, @Name, '', @Dept, @Reason, @CardNo, @DinnerType, '5', '2', '', 'T4', 1, @Member, @CardNoAgain)", sqc, transaction);

            cmd.Parameters.AddWithValue("@Id", idtmp);
            cmd.Parameters.AddWithValue("@Date", startDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Name", txtName.Text);
            cmd.Parameters.AddWithValue("@Dept", comboBox1.Text + "-團體");
            cmd.Parameters.AddWithValue("@Reason", txtReason.Text);
            cmd.Parameters.AddWithValue("@CardNo", txtCardNo.Text);
            cmd.Parameters.AddWithValue("@DinnerType", Dtype);
            cmd.Parameters.AddWithValue("@Member", txtMember.Text);
            cmd.Parameters.AddWithValue("@CardNoAgain", txtCardNo.Text);

            cmd.ExecuteNonQuery(); // 確保這裡的連接已經打開
        }

        private void InsertBaseBookData(SqlConnection sqc, SqlTransaction transaction, string idtmp, DateTime startDate, int Dtype)
        {
            SqlCommand baseBookCmd = new SqlCommand("INSERT INTO BaseBook (ID, BDate, DType, CardNo, UserID, Name, Department, Flag, BNum, DNum, DinnerArea, Expense, dbs, chkup) " +
                                                    "VALUES (@Id, @Date, @DinnerType, @CardNo, '55555', @Reason, @Name, 5, @Member, 0, 2, 0, 'T4', 0)", sqc, transaction);

            baseBookCmd.Parameters.AddWithValue("@Id", idtmp);
            baseBookCmd.Parameters.AddWithValue("@Date", startDate.ToString("yyyy-MM-dd"));
            baseBookCmd.Parameters.AddWithValue("@DinnerType", Dtype);
            baseBookCmd.Parameters.AddWithValue("@CardNo", txtCardNo.Text);
            baseBookCmd.Parameters.AddWithValue("@Name", txtName.Text);
            baseBookCmd.Parameters.AddWithValue("@Reason", txtReason.Text);
            baseBookCmd.Parameters.AddWithValue("@Member", txtMember.Text);

            baseBookCmd.ExecuteNonQuery(); // 執行命令
        }

        private void InsertBookDinnerLogData(SqlConnection sqc, SqlTransaction transaction, string idtmp, DateTime startDate, int Dtype)
        {
            SqlCommand BDLogCmd = new SqlCommand("INSERT INTO [db_owner].[BookDinnerLog] (loguser, logtime, logapps, username, dbs, Dtype, CardNo, changeaddease, changedate, Num2) " +
                                                 "VALUES (@UserID, @LogDate,'BookVisiter3', @Reason, 'T4', @DinnerType, @CardNo, 1, @Date, @Member)", sqc, transaction);

            BDLogCmd.Parameters.AddWithValue("@UserID", txtUserId.Text);
            BDLogCmd.Parameters.AddWithValue("@LogDate", DateTime.Now.ToString("yyyy-MM-dd") + " 08:45:15");
            BDLogCmd.Parameters.AddWithValue("@Reason", txtReason.Text);
            BDLogCmd.Parameters.AddWithValue("@DinnerType", Dtype);
            BDLogCmd.Parameters.AddWithValue("@CardNo", txtCardNo.Text);
            BDLogCmd.Parameters.AddWithValue("@Date", startDate.ToString("yyyy-MM-dd"));
            BDLogCmd.Parameters.AddWithValue("@Member", txtMember.Text);

            BDLogCmd.ExecuteNonQuery();  // 執行命令
        }

        private void InsertBookDinnerGPData(SqlConnection sqc, SqlTransaction transaction, string idtmp, int memberCount)
        {
            for (int j = memberCount; j > 0; j--)
            {
                SqlCommand BDGPCmd = new SqlCommand("INSERT INTO [db_owner].[BookDinnerGP] (ID, CardNo, Name, NameID, dbs) " +
                                                    "VALUES (@ID, @CardNo, '1', @NameID, 'T4' )", sqc, transaction);
                BDGPCmd.Parameters.AddWithValue("@ID", idtmp);
                BDGPCmd.Parameters.AddWithValue("@CardNo", txtCardNo.Text);
                BDGPCmd.Parameters.AddWithValue("@NameID", j);

                BDGPCmd.ExecuteNonQuery();  // 執行命令
            }
        }

        private void DVInsertBaseBookData(SqlConnection sqc, SqlTransaction transaction, string idtmp, DateTime startDate, int Dtype)
        {
            SqlCommand baseBookCmd = new SqlCommand("INSERT INTO BaseBook (ID, BDate, DType, CardNo, UserID, Name, Department, Flag, BNum, DNum, DinnerArea, Expense, dbs, chkup) " +
                                                   "VALUES (@Id, @Date, @DinnerType, @CardNo, '55555', @Reason, @Name, 5, @Member, @Member, 3, 0, 'T4', 0)", sqc, transaction);

            baseBookCmd.Parameters.AddWithValue("@Id", idtmp);
            baseBookCmd.Parameters.AddWithValue("@Date", startDate.ToString("yyyy-MM-dd"));
            baseBookCmd.Parameters.AddWithValue("@DinnerType", Dtype);
            baseBookCmd.Parameters.AddWithValue("@CardNo", txtCardNo.Text);
            baseBookCmd.Parameters.AddWithValue("@Name", txtName.Text);
            baseBookCmd.Parameters.AddWithValue("@Reason", txtReason.Text);
            baseBookCmd.Parameters.AddWithValue("@Member", txtMember.Text);

            baseBookCmd.ExecuteNonQuery(); // 執行命令
        }

        private void DVInsertBookDinnerLogData(SqlConnection sqc, SqlTransaction transaction, string idtmp, DateTime startDate, int Dtype)
        {
            SqlCommand BDLogCmd = new SqlCommand("INSERT INTO [db_owner].[BookDinnerLog] (loguser, logtime, logapps, username, dbs, Dtype, CardNo, changeaddease, changedate, Num2) " +
                                                 "VALUES (@UserID, @LogDate,'BookVisiter5', @Reason, 'T4', @DinnerType, @CardNo, 1, @Date, @Member)", sqc, transaction);

            BDLogCmd.Parameters.AddWithValue("@UserID", txtUserId.Text);
            BDLogCmd.Parameters.AddWithValue("@LogDate", DateTime.Now.ToString("yyyy-MM-dd") + " 08:45:15");
            BDLogCmd.Parameters.AddWithValue("@Reason", txtReason.Text);
            BDLogCmd.Parameters.AddWithValue("@DinnerType", Dtype);
            BDLogCmd.Parameters.AddWithValue("@CardNo", txtCardNo.Text);
            BDLogCmd.Parameters.AddWithValue("@Date", startDate.ToString("yyyy-MM-dd"));
            BDLogCmd.Parameters.AddWithValue("@Member", txtMember.Text);

            BDLogCmd.ExecuteNonQuery();  // 執行命令
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            // 取得使用者輸入的卡號和日期範圍
            string cardNo = txtCardNo.Text; // 假設 txtCardNo 是 TextBox 控制項
            DateTime startDate = dateTimePicker1.Value;  // 假設 dateTimePicker1 是開始日期控件
            DateTime endDate = dateTimePicker2.Value;    // 假設 dateTimePicker2 是結束日期控件

            // 檢查使用者是否輸入有效資料
            if (string.IsNullOrWhiteSpace(cardNo) || txtCardNo.Text.Length < 10)
            {
                MessageBox.Show("請輸入正確的卡號！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 檢查開始日期和結束日期是否選擇有效日期
            if (startDate.Date < DateTime.Now.Date || endDate.Date < DateTime.Now.Date)
            {
                MessageBox.Show("請選擇有效的開始日期!(日期不能小於今天)", "日期錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // 如果未選擇有效日期，則退出
            }

            if (startDate.Date > endDate.Date)
            {
                MessageBox.Show("結束日期不能早於開始日期！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            string query = @"SELECT TOP(1) * FROM BookDinner
                     WHERE CardNo = @cardno 
                     AND DDate BETWEEN @StartDate AND @EndDate";

            using (SqlConnection connection = DatabaseHelper.GetConnection())  // 假設 DatabaseHelper 是你管理資料庫連線的類別
            {
                try
                {
                    // 開啟連線
                    connection.Open();

                    // 建立 SQL 命令並加入參數
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@cardno", cardNo);
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));

                        // 執行查詢並檢查是否有資料
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            // 資料庫中有符合條件的資料
                            MessageBox.Show("該卡號在指定日期範圍內已有資料！", "資料已存在", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            //開啟資訊填寫
                            txtUserId.Enabled = true;
                            txtName.Enabled = true;
                            txtMember.Enabled = true;
                            txtReason.Enabled = true;
                            comboBox1.Enabled = true;
                            DinnerType.Enabled = true;
                            button1.Enabled = true;
                            //鎖住日期框,卡號
                            dateTimePicker1.Enabled = false;
                            dateTimePicker2.Enabled = false;
                            txtCardNo.Enabled = false;
                            btnCheck.Enabled = false;

                            // 資料庫中沒有符合條件的資料
                            //MessageBox.Show("該卡號在指定日期範圍內無資料！", "資料不存在", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // 根據卡號進行特定處理
                            if (cardNo == "0257964541")
                            {
                                MessageBox.Show("該卡號為配送團體卡!", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                // 設定 DinnerType 為 "晚餐"
                                DinnerType.SelectedItem = "晚餐";

                                // 清除 comboBox1 的項目並加入新的選項
                                comboBox1.Items.Clear();
                                comboBox1.Items.Add("配送交貨");
                                comboBox1.Items.Add("配送成品");

                                // 如果已經有選項的話，選擇一個默認項目
                                comboBox1.SelectedIndex = 0;  // 假設選擇 "配送交貨" 為默認值
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"資料庫操作錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

    }
}


