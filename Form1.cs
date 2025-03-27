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
    public partial class Form1 : Form
    {


        /// <summary>
        /// 2025-01-08 : DataGridView裡面元件控制判斷邏輯
        /// </summary>
        public Form1()
        {
            InitializeComponent();


            toolStripStatusLabel1.Text = "資料庫:正式區";
            toolStripStatusLabel2.Text = "Ver.1.0.0";
            toolStripStatusLabel3.Text = "Release Date:2025-01-16";

            // 確保在此處綁定事件，讓表單加載時進行初始化
            this.Load += new EventHandler(Form1_Load);
        }

        // 當表單加載時，設置所有 TextBox 的預設提示文字
        private void Form1_Load(object sender, EventArgs e)
        {
            // 設定初始按鈕狀態
            UpdateButtonState();

            //下拉選單(餐別)
            comboBox1.Items.Add("中餐"); comboBox1.Items.Add("晚餐");//增加下拉選單的項目
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;// 只允許選擇項目，不能輸入其他內容
            comboBox1.SelectedItem = "中餐";// 設置選中項目為 "中餐"


            // 設置每個 TextBox 的提示文字並設置顏色為灰色
            foreach (Control control in this.Controls)
            {
                // 使用 as 來轉換為 TextBox
                TextBox textBox = control as TextBox;

                // 確保 control 轉換為 TextBox 且不為 null
                if (textBox != null)
                {
                    // 根據 TextBox 的名稱來設置 Tag 屬性
                    if (textBox.Name == "txtCardNo") textBox.Tag = "請輸入要查詢的卡號";

                    // 設置 TextBox 顯示 Tag 作為提示文字
                    textBox.Text = textBox.Tag.ToString();  // 設置提示文字
                    textBox.ForeColor = System.Drawing.Color.Gray;  // 設置顏色為灰色

                    // 綁定 Enter 和 Leave 事件
                    textBox.Enter += new EventHandler(txtBox_Enter);
                    textBox.Leave += new EventHandler(txtBox_Leave);
                }
            }
#if DEBUG
            toolStripStatusLabel1.Text = "測試區";
#endif
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

        // 恢復提示文字狀態
        private void ResetTextBoxPlaceholder(TextBox textBox)
        {
            txtCardNo.Enabled = true;
            txtCardNo.Text = "";

            if (textBox != null && textBox.Text.Length == 0)
            {
                textBox.Text = textBox.Tag.ToString();  // 恢復提示文字
                textBox.ForeColor = System.Drawing.Color.Gray;  // 設置顏色為灰色
            }
        }

        //開啟新增團體訪客訂餐
        private void btnTeamVT_Click(object sender, EventArgs e)
        {
            // 創建 Form2 的實例並顯示它
            TeamVT teamvt = new TeamVT();
            btnClear_Click(null, EventArgs.Empty);// 調用 btnClear 觸發事件，清空資料
            teamvt.ShowDialog();  // 這會顯示 TeamVT 且阻止回到 Form1
        }

        //團體查詢鍵
        private void btnSelect_Click(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            txtCardNo.Enabled = false;
            chkEdit.Checked = false;
            chkDelete.Checked = false;

            if (txtCardNo.Text == null || txtCardNo.Text.Length < 10)
            {
                MessageBox.Show("請輸入正確的卡號", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // 恢復初始提示文字
                ResetTextBoxPlaceholder(txtCardNo);
                comboBox1.Enabled = true;
                return;  // 防止繼續執行以下代碼
            }

            DateTime startDate = dateTimePicker1.Value;  // 獲取開始日期
            DateTime endDate = dateTimePicker2.Value;    // 獲取結束日期


            // 檢查開始日期和結束日期是否選擇有效日期
            if (startDate < DateTime.Now.Date || endDate < DateTime.Now.Date)
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

            long elapsedTicks = endDate.Ticks - startDate.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            int alldays = elapsedSpan.Days;  // 獲取開始日期到結束日期之間的天數

            int Dtype = 0;

            // 確保 SelectedItem 不為 null (餐別)
            if (comboBox1.SelectedItem != null)
            {
                if (comboBox1.SelectedItem.ToString() == "中餐")
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

            // CardNo 
            string cardNo = txtCardNo.Text;

            // 使用提取出來的 LoadData 方法
            LoadData(cardNo, startDate, endDate, Dtype);


        }

        //查詢顯示團體Load
        private void LoadData(string cardNo, DateTime startDate, DateTime endDate, int Dtype)
        {
            // 建構 SQL 查詢語句
            string query = @"SELECT bd.ID, bl.loguser, bd.CardNo, bd.DDate, bd.BeVisit, bd.Department, bd.Reason, bd.Num2, bd.dbs
                             FROM BookDinner bd
                             JOIN [db_owner].[BookDinnerLog] bl ON bd.CardNo = bl.CardNo AND bd.DDate = bl.changedate
                             WHERE bd.CardNo = @CardNo 
                             AND bd.DDate BETWEEN @StartDate AND @EndDate
                             AND bd.DType = @Dtype";

            // 這裡會根據編譯模式選擇正確的連線字串
            using (SqlConnection sqc = DatabaseHelper.GetConnection())
            {
                try
                {
                    sqc.Open();  // 確保連接被打開    

                    // 創建 SqlCommand，傳遞查詢語句和連接
                    using (SqlCommand cmd = new SqlCommand(query, sqc))
                    {
                        // 使用參數化查詢來避免 SQL 注入
                        cmd.Parameters.AddWithValue("@CardNo", cardNo);
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));  // 起始日期
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));  // 結束日期
                        cmd.Parameters.AddWithValue("@Dtype", Dtype);


                        // 創建一個 DataTable 來保存查詢結果
                        DataTable dataTable = new DataTable();

                        // 使用 SqlDataAdapter 來填充 DataTable
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }

                        // 將 DataTable 設置為 DataGridView 的 DataSource
                        dataGridView1.DataSource = dataTable;


                        dataGridView1.Columns["ID"].Visible = false;  // 隱藏ID列
                        dataGridView1.Columns["dbs"].Visible = false;  // 隱藏dbs列

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show($"執行資料庫時出現錯誤: {ex.Message}", "錯誤");
                    return;
                }
            }

            // 加載資料後，更新按鈕狀態
            UpdateButtonState();
        }

        //修改勾選
        private void chkEdit_CheckedChanged(object sender, EventArgs e)
        {
            // 根據 chkEdit 的狀態更新 "修改" 按鈕的顯示名稱
            if (chkEdit.Checked)
            {
                chkDelete.Enabled = false;
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;

                // 勾選了修改，更新按鈕文字為 "修改"
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells["ActionButton"].Value = "修改";
                }
            }
            else
            {
                chkDelete.Enabled = true;
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells["ActionButton"].Value = "";
                }
            }
        }

        //刪除勾選
        private void chkDelete_CheckedChanged(object sender, EventArgs e)
        {
            // 根據 chkDelete 的狀態更新 "刪除" 按鈕的顯示名稱
            if (chkDelete.Checked)
            {
                chkEdit.Enabled = false;
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;

                // 勾選了修改，更新按鈕文字為 "修改"
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells["ActionButton"].Value = "刪除";
                }
            }
            else
            {
                chkEdit.Enabled = true;
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;

                // 勾選了修改，更新按鈕文字為 "修改"
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells["ActionButton"].Value = "";
                }
            }
        }

        //清除鍵
        private void btnClear_Click(object sender, EventArgs e)
        {

            // 清空 DataGridView 內的數據，但保留欄位名稱
            if (dataGridView1.DataSource != null)
            {
                // 如果 DataGridView 綁定了資料源，將資料源設為 null，然後重新設定資料表結構
                DataTable dataTable = (DataTable)dataGridView1.DataSource;
                dataTable.Clear(); // 清空 DataTable 的所有資料               
            }
            else
            {
                // 如果 DataGridView 沒有資料源，則清空所有行但保留欄位結構
                dataGridView1.Rows.Clear();
            }

            // 重置其他 UI 控件
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            dateTimePicker1.Enabled = true;
            dateTimePicker2.Enabled = true;
            comboBox1.SelectedItem = "中餐";
            comboBox1.Enabled = true;
            ResetTextBoxPlaceholder(txtCardNo);
            chkEdit.Checked = false;
            chkEdit.Enabled = false;
            chkDelete.Checked = false;
            chkDelete.Enabled = false;

            // 更新按鈕狀態
            UpdateButtonState();
        }

        //修改&刪除 勾選框
        private void UpdateButtonState()
        {

            // 當 DataGridView 的 DataSource 為 null 或資料來源為空時禁用 chkEdit 和 chkDelete 按鈕
            if (dataGridView1.DataSource == null || ((DataTable)dataGridView1.DataSource).Rows.Count == 0)
            {
                chkEdit.Enabled = false;
                chkEdit.Checked = false;
                chkDelete.Enabled = false;
                chkDelete.Checked = false;
            }
            else
            {
                // 當 DataGridView 有資料時，檢查 chkEdit 和 chkDelete 狀態
                chkEdit.Enabled = true;
                chkEdit.Checked = false;
                chkDelete.Enabled = true;
                chkDelete.Checked = false;
            }
        }

        //加載資料
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // 如果 DataGridView 沒有資料，更新按鈕狀態
            if (dataGridView1.Rows.Count == 0)
            {
                UpdateButtonState();
            }
        }

        //資料view
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 確保 DataGridView 沒有資料無法操作
            if (dataGridView1.DataSource == null || ((DataTable)dataGridView1.DataSource).Rows.Count == 0)
            {
                MessageBox.Show("目前沒有資料，無法進行操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;// 如果發生錯誤，不再繼續後面的操作
            }

            // 1.確保點擊的是 DataGridViewCheckBoxColumn(資料裡面的勾選框)
            if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0)
            {
                // 獲取點擊的行和列
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                bool isChecked = (bool)(row.Cells[e.ColumnIndex].Value ?? false);

                // 反轉選擇狀態
                row.Cells[e.ColumnIndex].Value = !isChecked;

                // 顯示反轉後的狀態
                //MessageBox.Show($"第 {e.RowIndex + 1} 行 : 日期為 {row.Cells[5].Value} 已被點擊，新狀態為：{!isChecked}");

            }

            // 2.如果點擊的是 ButtonColumn (按鈕列)
            if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {

                // 獲取點擊的行
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // 檢查對應的 CheckBox 是否已選中（CheckBox 是第一列）
                bool isChecked = (bool)(row.Cells["ChkColumn"].Value ?? false); // "SelectColumn" 是 CheckBoxColumn 的名稱

                if (!isChecked)
                {
                    // 如果未勾選，提示用戶並退出操作
                    MessageBox.Show($"請先勾選第 {e.RowIndex + 1} 行，才能執行按鈕操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;// 如果發生錯誤，不再繼續後面的操作
                }
                if (!chkEdit.Checked && !chkDelete.Checked)
                {
                    // 如果未勾選，提示用戶並退出操作 (修改或刪除)
                    MessageBox.Show("請先勾選'修改'或'刪除'", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;// 如果發生錯誤，不再繼續後面的操作
                }

                // 獲取資料庫中的 logapps 值
                string cardNo = row.Cells["CardNo"].Value?.ToString(); // 假設 CardNo 是資料列中的欄位名稱
                string date = row.Cells["DDate"].Value?.ToString();
                string logappsValue = null;

                using (SqlConnection conn = DatabaseHelper.GetConnection()) // 請確保您已經有適當的 DatabaseHelper 類別
                {
                    try
                    {
                        conn.Open();
                        string query = @"SELECT logapps FROM [db_owner].[BookDinnerLog] WHERE CardNo = @CardNo AND changedate = @date";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@CardNo", cardNo);
                            cmd.Parameters.AddWithValue("@date", date);

                            // 執行查詢並讀取結果
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                logappsValue = result.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"查詢資料庫時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // 查詢出錯時退出操作
                    }
                }

                // 如果 logapps 為 "BookVisiter1"，提示用戶並退出操作
                if (logappsValue == "BookVisiter1")
                {
                    MessageBox.Show("這是個人訪客資料，請至個人系統操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    DateTime startDate = dateTimePicker1.Value;  // 獲取開始日期
                    DateTime endDate = dateTimePicker2.Value;    // 獲取結束日期
                    int Dtype = 0;
                    if (comboBox1.SelectedItem != null)
                    {
                        if (comboBox1.SelectedItem.ToString() == "中餐")
                        {
                            Dtype = 2;
                        }
                        else
                        {
                            Dtype = 3;
                        }
                    }
                    // CardNo 
                    string cardno = txtCardNo.Text;
                    LoadData(cardno, startDate, endDate, Dtype);

                    return; // 終止後續操作
                }

                // 根據勾選的狀態進行對應操作
                if (chkEdit.Checked)
                {

                    UpdateDatabase(e.RowIndex);
                }
                if (chkDelete.Checked)
                {
                    DeleteDatabase(e.RowIndex);
                }


            }





        }
        //修改團體訂餐資料(目前只有說明與人數可做修改)
        private void UpdateDatabase(int rowIndex)
        {
            // 從 DataGridView 中獲取修改後的資料
            var id = dataGridView1.Rows[rowIndex].Cells["ID"].Value.ToString(); // 獲取 ID
            var cardNo = dataGridView1.Rows[rowIndex].Cells["CardNo"].Value.ToString();
            var dDate = dataGridView1.Rows[rowIndex].Cells["DDate"].Value.ToString();
            var num2 = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["Num2"].Value);
            var reason = dataGridView1.Rows[rowIndex].Cells["Reason"].Value.ToString();

            // 判斷 num2 是否小於 1，若小於則顯示錯誤訊息並退出
            if (num2 <= 1)
            {
                MessageBox.Show("人數必須大於 1", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  // 退出方法，避免執行後續操作
            }

            // 建立更新的 SQL 查詢
            // 更新 BookDinner 的資料
            string updateBookDinnerQuery = @"
        UPDATE BookDinner
        SET Num2 = @Num2, Reason = @Reason
        WHERE CardNo = @CardNo AND DDate = @DDate AND ID = @ID";

            // 更新 BaseBook 的資料
            string updateBaseBookQuery = @"
        UPDATE BaseBook
        SET BNum = @Num2, Name = @Reason
        WHERE CardNo = @CardNo AND BDate = @DDate AND ID = @ID";

            // 更新 BookDinnerLog 的資料
            string updateLogQuery = @"
        UPDATE [db_owner].[BookDinnerLog]
        SET Num2 = @Num2, username = @Reason
        WHERE CardNo = @CardNo AND changedate = @DDate";

            // 刪除 BookDinnerGP 的資料
            string deleteBookDinnerGPQuery = @"
        DELETE FROM [db_owner].[BookDinnerGP]
        WHERE ID = @ID AND CardNo = @CardNo";

            // 插入新的 BookDinnerGP 資料
            string insertBookDinnerGPQuery = @"
        INSERT INTO [db_owner].[BookDinnerGP] (ID, CardNo, Name, NameID, dbs)
        VALUES (@ID, @CardNo, @Name, @NameID, @dbs)";

            using (SqlConnection sqc = DatabaseHelper.GetConnection())
            {
                try
                {
                    sqc.Open();  // 打開資料庫連接

                    using (SqlTransaction transaction = sqc.BeginTransaction())  // 使用事務確保多個操作的一致性
                    {
                        try
                        {
                            // 更新 BookDinner
                            using (SqlCommand cmd = new SqlCommand(updateBookDinnerQuery, sqc, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                                cmd.Parameters.AddWithValue("@DDate", dDate);
                                cmd.Parameters.AddWithValue("@Num2", num2);
                                cmd.Parameters.AddWithValue("@Reason", reason);
                                cmd.ExecuteNonQuery();
                            }

                            // 更新 BaseBook
                            using (SqlCommand cmd = new SqlCommand(updateBaseBookQuery, sqc, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                                cmd.Parameters.AddWithValue("@DDate", dDate);
                                cmd.Parameters.AddWithValue("@Num2", num2);
                                cmd.Parameters.AddWithValue("@Reason", reason);
                                cmd.ExecuteNonQuery();
                            }

                            // 記錄操作到 BookDinnerLog
                            using (SqlCommand cmd = new SqlCommand(updateLogQuery, sqc, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                                cmd.Parameters.AddWithValue("@DDate", dDate);
                                cmd.Parameters.AddWithValue("@Reason", reason);
                                cmd.Parameters.AddWithValue("@Num2", num2);
                                cmd.Parameters.AddWithValue("@ChangedDate", DateTime.Now);  // 記錄變更時間
                                cmd.ExecuteNonQuery();
                            }

                            // 如果卡號為 "0257964541"，僅執行更新語句
                            if (cardNo == "0257964541")
                            {
                                transaction.Commit();
                                MessageBox.Show("資料更新成功（卡號：0257964541）", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            // 刪除 BookDinnerGP 資料
                            using (SqlCommand cmd = new SqlCommand(deleteBookDinnerGPQuery, sqc, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                                cmd.ExecuteNonQuery();
                            }

                            // 根據 Num2 插入新的資料到 BookDinnerGP
                            for (int i = num2; i >= 1; i--)
                            {
                                using (SqlCommand cmd = new SqlCommand(insertBookDinnerGPQuery, sqc, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ID", id);
                                    cmd.Parameters.AddWithValue("@CardNo", cardNo);
                                    cmd.Parameters.AddWithValue("@Name", "1");
                                    cmd.Parameters.AddWithValue("@dbs", "T4"); // 假設 dbs 為 "T4"
                                    cmd.Parameters.AddWithValue("@NameID", i);  // 插入遞減的 NameID
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 提交事務，確保所有操作都成功
                            transaction.Commit();

                            MessageBox.Show("資料更新成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            // 發生錯誤時，回滾事務
                            transaction.Rollback();
                            MessageBox.Show($"更新過程中出現錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;  // 如果發生錯誤，不再繼續後面的操作
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"資料庫連接或操作錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;  // 如果發生錯誤，不再繼續後面的操作
                }
            }
            //刷新頁面
            int Dtype = 0;
            if (comboBox1.SelectedItem != null)
            {
                if (comboBox1.SelectedItem.ToString() == "中餐")
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
                MessageBox.Show("發生餐別讀取錯誤", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  // 如果沒有選擇餐點類型，則退出方法
            }
            LoadData(txtCardNo.Text, dateTimePicker1.Value, dateTimePicker2.Value, Dtype);
        }

        //刪除團體訂餐資料
        private void DeleteDatabase(int rowIndex)
        {
            // 從 DataGridView 中獲取要刪除的資料
            var id = dataGridView1.Rows[rowIndex].Cells["ID"].Value.ToString(); // 獲取 ID
            var cardNo = dataGridView1.Rows[rowIndex].Cells["CardNo"].Value.ToString();
            var dDate = dataGridView1.Rows[rowIndex].Cells["DDate"].Value.ToString();

            // 建立刪除的 SQL 查詢語句
            string deleteBookDinnerQuery = @"
    DELETE FROM BookDinner
    WHERE CardNo = @CardNo AND DDate = @DDate AND ID = @ID";

            string deleteBaseBookQuery = @"
    DELETE FROM BaseBook
    WHERE CardNo = @CardNo AND BDate = @DDate AND ID = @ID";

            string deleteLogQuery = @"
    DELETE FROM [db_owner].[BookDinnerLog]
    WHERE CardNo = @CardNo AND changedate = @DDate";

            string deleteBookDinnerGPQuery = @"
    DELETE FROM [db_owner].[BookDinnerGP]
    WHERE ID = @ID";

            using (SqlConnection sqc = DatabaseHelper.GetConnection())
            {
                try
                {
                    sqc.Open();  // 打開資料庫連接

                    using (SqlTransaction transaction = sqc.BeginTransaction())  // 使用事務確保多個操作的一致性
                    {
                        try
                        {
                            // 刪除 BookDinner
                            using (SqlCommand cmd = new SqlCommand(deleteBookDinnerQuery, sqc, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                                cmd.Parameters.AddWithValue("@DDate", dDate);
                                cmd.ExecuteNonQuery();
                            }

                            // 刪除 BaseBook
                            using (SqlCommand cmd = new SqlCommand(deleteBaseBookQuery, sqc, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                                cmd.Parameters.AddWithValue("@DDate", dDate);
                                cmd.ExecuteNonQuery();
                            }

                            // 刪除 BookDinnerLog
                            using (SqlCommand cmd = new SqlCommand(deleteLogQuery, sqc, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                                cmd.Parameters.AddWithValue("@DDate", dDate);
                                cmd.ExecuteNonQuery();
                            }

                            // 刪除 BookDinnerGP
                            using (SqlCommand cmd = new SqlCommand(deleteBookDinnerGPQuery, sqc, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.ExecuteNonQuery();
                            }

                            // 提交事務，確保所有操作都成功
                            transaction.Commit();

                            MessageBox.Show("資料刪除成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            // 發生錯誤時，回滾事務
                            transaction.Rollback();
                            MessageBox.Show($"刪除過程中出現錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;  // 如果發生錯誤，不再繼續後面的操作
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"資料庫連接或操作錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;  // 如果發生錯誤，不再繼續後面的操作
                }
            }
            //刷新頁面
            int Dtype = 0;
            if (comboBox1.SelectedItem != null)
            {
                if (comboBox1.SelectedItem.ToString() == "中餐")
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
                MessageBox.Show("發生餐別讀取錯誤", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  // 如果沒有選擇餐點類型，則退出方法
            }
            LoadData(txtCardNo.Text, dateTimePicker1.Value, dateTimePicker2.Value, Dtype);
        }


    }
}
