using Ionic.Zip;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MailKit;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using MimeKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailRat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 郵件搜尋
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Decrypt_Click(object sender, EventArgs e)
        {
            //清空顯示
            ShowOutput.Visible = false;
            ShowOutput.Text = string.Empty;

            //檢查輸入欄位是否都有資料
            CheckInput();

            //連結變數
            string email = EmailAccount.Text;
            string passwordd = Password.Text;
            string filepassword = PDFPassword.Text;

            //從app.Config提取目標時間，只有在這個時間之後收到的郵件才會進行判斷
            DateTime TargetDateTime = DateTime.Parse(ConfigurationManager.AppSettings["SetDateTime"]);
            //在設置一個變數用來儲存每一封信的收信時間，最後處理的那封信的時間會存回app.Config中，成為下一次處理的目標時間
            DateTime SetNextDateTime = new DateTime();

            //建立處理用資料夾
            string newFolderPath = BuildTempFolder();

            //打開Mail Server閘門
            var client = new ImapClient();
            string[] emailaccount = email.Split('@');

            try
            {
                //登入
                client.Connect("mail.asiavista.com.tw", 993, SecureSocketOptions.SslOnConnect);
                client.Authenticate(emailaccount[0], passwordd);
            }
            catch (Exception ex)
            {
                ShowOutput.Text = "帳號or密碼錯誤";
                return;
            }

            // 取得收件匣
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);

            // 取得收件匣中的郵件，設定條件是指定日期之後
            var messages = inbox.Search(SearchQuery.DeliveredAfter(TargetDateTime));

            // 迭代所有郵件
            foreach (var message in messages)
            {
                try
                {
                    //取得該郵件資訊
                    var mimeMessage = inbox.GetMessage(message);

                    // 取得附件
                    var attachments = mimeMessage.Attachments.ToList();

                    //設置multipart變數將原先郵件的Body存起來，用來儲存後續更動
                    Multipart multipart = mimeMessage.Body as Multipart;

                    //設置變數存放信件收到的時間
                    SetNextDateTime = mimeMessage.Date.LocalDateTime;

                    //檢查是否有檔案進入處理
                    bool check = false;

                    #region 檢查並處理郵件中的附件檔案
                    //再次確認信件的日期和時間
                    //mimeMessage.Date.LocalDateTime > TargetDateTime
                    if (mimeMessage.Date.LocalDateTime > TargetDateTime)
                    {
                        // 迭代所有附件
                        foreach (var attachment in attachments)
                        {
                            //PDF、Excel、Word能不能開啟
                            bool state = false;

                            // 判斷附件是否為PDF、Excel、Word、zip
                            if (attachment is MimePart mimePart)
                            {
                                //判斷PDF
                                if (mimePart.ContentType?.ToString().Contains("application/pdf") == true)
                                {
                                    //判斷是不是PDF檔案，再判斷能不能開啟，無法開啟代表有加密，將加密的檔案加入預設的字料夾當中，執行解密並刪除原來的加密檔案，將名稱變回原來的樣子，加入到資料夾當中
                                    if (mimePart.ContentDisposition?.FileName != null && mimePart.ContentDisposition.FileName.Contains(".pdf"))
                                    {
                                        string filePath = Path.Combine(newFolderPath, attachment.ContentDisposition.FileName);
                                        using (var stream = File.Create(filePath))
                                        {
                                            ((MimePart)attachment).Content.DecodeTo(stream);
                                        }

                                        try
                                        {
                                            using (var trytoopen = new iTextSharp.text.pdf.PdfReader(filePath))
                                            {
                                                state = true;
                                            }
                                        }
                                        catch
                                        {
                                            state = false;
                                        }

                                        //True代表PDF可以開啟就刪掉，只處理不能開的，就是有加密的
                                        if (state)
                                        {
                                            File.Delete(filePath);
                                            break;
                                        }
                                        else  //False代表PDF無法開啟，要進行解密，解密完把原始檔案刪除
                                        {
                                            try
                                            {
                                                //PDF解密
                                                PDFDecrypt(newFolderPath, mimePart.ContentDisposition.FileName, filepassword);

                                                // 刪除原始檔案
                                                File.Delete(filePath);
                                                //移除郵件中的附件
                                                multipart.Remove(attachment);
                                                mimeMessage.Body = multipart;
                                                //要寫入新的附件
                                                check = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                ShowOutput.Text = "解密密碼錯誤";
                                                Directory.Delete(newFolderPath, true);
                                                return;
                                            }
                                        }
                                    }
                                }

                                //判斷Excel
                                else if (mimePart.ContentType?.ToString().Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") == true)
                                {
                                    if (mimePart.ContentDisposition?.FileName != null && mimePart.ContentDisposition.FileName.Contains(".xlsx"))
                                    {
                                        string filePath = System.IO.Path.Combine(newFolderPath, attachment.ContentDisposition.FileName);

                                        using (var stream = File.Create(filePath))
                                        {
                                            ((MimePart)attachment).Content.DecodeTo(stream);
                                        }

                                        //判斷目標Excel檔案能不能開啟
                                        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application()
                                        {
                                            Visible = false
                                        };
                                        try
                                        {
                                            Workbook workbook = excel.Workbooks.Open(filePath, ReadOnly: true, Password: "", IgnoreReadOnlyRecommended: true, Notify: false);
                                            workbook.Close(SaveChanges: false);
                                            state = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            state = false;
                                        }
                                        excel.Quit();

                                        if (state)
                                        {
                                            File.Delete(filePath);
                                            break;
                                        }
                                        else  //False代表Excel無法開啟，要進行解密，解密完直接塞進那封郵件裡
                                        {
                                            try
                                            {
                                                ExcelDecrypt(newFolderPath, mimePart.ContentDisposition.FileName, filepassword);

                                                // 刪除原始檔案
                                                File.Delete(filePath);
                                                multipart.Remove(attachment);
                                                mimeMessage.Body = multipart;
                                                check = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                ShowOutput.Text = "解密密碼錯誤";
                                                Directory.Delete(newFolderPath, true);
                                                return;
                                            }
                                        }
                                    }
                                }

                                //判斷Word
                                else if (mimePart.ContentType?.ToString().Contains("application/vnd.openxmlformats-officedocument.wordprocessingml.document") == true)
                                {
                                    if (mimePart.ContentDisposition?.FileName != null && mimePart.ContentDisposition.FileName.Contains(".docx"))
                                    {
                                        string filePath = System.IO.Path.Combine(newFolderPath, attachment.ContentDisposition.FileName);

                                        using (var stream = File.Create(filePath))
                                        {
                                            ((MimePart)attachment).Content.DecodeTo(stream);
                                        }


                                        //可以判斷Word是否要密碼才能開啟
                                        Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
                                        //判斷目標Word檔案能不能開啟
                                        try
                                        {
                                            // 打開文件
                                            Document doc = app.Documents.Open(filePath, ReadOnly: true, PasswordDocument: " ",
                                            Visible: false, OpenAndRepair: false, NoEncodingDialog: true);
                                            doc.Close(SaveChanges: false);
                                            state = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            state = false;
                                        }
                                        app.Quit();

                                        if (state)
                                        {
                                            File.Delete(filePath);
                                            break;
                                        }
                                        else  //False代表Excel無法開啟，要進行解密，解密完直接塞進那封郵件裡
                                        {
                                            try
                                            {
                                                WordDecrypt(newFolderPath, mimePart.ContentDisposition.FileName, filepassword);

                                                // 刪除原始檔案
                                                File.Delete(filePath);
                                                multipart.Remove(attachment);
                                                mimeMessage.Body = multipart;
                                                check = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                ShowOutput.Text = "解密密碼錯誤";
                                                Directory.Delete(newFolderPath, true);
                                                return;
                                            }
                                        }
                                    }
                                }

                                //判斷zip檔案
                                else if (mimePart.ContentType?.ToString().Contains("application/x-zip-compressed") == true || mimePart.ContentType?.ToString().Contains("application/zip") == true)
                                {
                                    if (mimePart.ContentDisposition?.FileName != null && mimePart.ContentDisposition.FileName.Contains(".zip"))
                                    {
                                        //設定下載路徑
                                        string filePath = System.IO.Path.Combine(newFolderPath, attachment.ContentDisposition.FileName);
                                        //將指定附件下載到指定路徑下
                                        using (var stream = File.Create(filePath))
                                        {
                                            ((MimePart)attachment).Content.DecodeTo(stream);
                                        }
                                        //設定解壓縮的路徑
                                        string newfolderName = "ForZip";
                                        string processZIP = Path.Combine(newFolderPath, newfolderName);
                                        Directory.CreateDirectory(processZIP);
                                        //檢查壓縮檔是否需要密碼才能解壓縮
                                        bool needPassword = false;

                                        using (var zip = ZipFile.Read(filePath))
                                        {
                                            needPassword = !ZipFile.CheckZipPassword(filePath, null);
                                        }
                                        //需要解壓縮needPassword會變成true，進入處理環節
                                        if (needPassword)
                                        {
                                            try
                                            {
                                                //ZIP壓縮檔解密處理
                                                ZIPDecrypt(newFolderPath, mimePart.ContentDisposition.FileName, filepassword, processZIP);
                                                //刪除原始檔案
                                                File.Delete(filePath);
                                                //附件移除
                                                multipart.Remove(attachment);
                                                mimeMessage.Body = multipart;
                                                //附件要更動
                                                check = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                ShowOutput.Text = "解密密碼錯誤";
                                                Directory.Delete(newFolderPath, true);
                                                return;
                                            }
                                        }
                                        else File.Delete(filePath);
                                    }
                                }
                            }
                        }

                        //解密檔案處理完畢，check會變成true，表示要從目標資料夾取出檔案加入附件當中
                        if (check)
                        {
                            //取得Temp資料夾下的所有檔案
                            string[] filePaths = Directory.GetFiles(newFolderPath);

                            //將所有檔案都用FileStream開啟
                            FileStream[] fileStreams = new FileStream[filePaths.Length];
                            for (int i = 0; i < filePaths.Length; i++)
                            {
                                fileStreams[i] = new FileStream(filePaths[i], FileMode.Open, FileAccess.Read);
                            }

                            int num = 0;
                            //開始讀取資料夾中的檔案，接著使用上面的FileStream
                            foreach (string fileName in filePaths)
                            {
                                var attachment = new MimePart()
                                {
                                    Content = new MimeContent(fileStreams[num]),
                                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                                    ContentTransferEncoding = ContentEncoding.Base64,
                                    FileName = System.IO.Path.GetFileName(fileName)
                                };

                                if (multipart != null) multipart.Add(attachment);

                                num++;
                            }

                            //設定新的Body
                            mimeMessage.Body = multipart;

                            //寫入郵件
                            inbox.Replace(message, mimeMessage);

                            //釋放FileStream占用的空間
                            foreach (var progress in fileStreams)
                            {
                                progress.Close();
                                progress.Dispose();
                            }

                            // 刪除每個檔案
                            foreach (string filePath in filePaths)
                            {
                                try
                                {
                                    File.Delete(filePath);
                                }
                                catch (Exception ex)
                                {
                                    ShowOutput.Text = "檔案刪除失敗，請手動刪除";
                                    return;
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    ShowOutput.Text = "解密失敗";
                    return;
                }
            }

            //把讀取到的最後一封信，通常是最新的一封信的時間寫回到app.Config中，下次抓日期就會從上次最後抓信的那個時間的下一封信開始處理郵件
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["SetDateTime"].Value = SetNextDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            // 斷開與郵件伺服器的連線
            client.Disconnect(true);

            //清空畫面
            EmailAccount.Text = string.Empty;
            Password.Text = string.Empty;
            PDFPassword.Text = string.Empty;
            ShowOutput.Visible = true;
            ShowOutput.Text = "解密完成";

            //刪除用來暫存PDF的資料夾
            if (Directory.Exists(newFolderPath)) Directory.Delete(newFolderPath, true);
        }

        /// <summary>
        /// 檢查三個接收控制項是否都有輸入
        /// </summary>
        private void CheckInput()
        {
            if (string.IsNullOrWhiteSpace(EmailAccount.Text))
            {
                MessageBox.Show("請輸入帳號。");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("請輸入密碼。");
                return;
            }

            if (string.IsNullOrWhiteSpace(PDFPassword.Text))
            {
                MessageBox.Show("請輸入解密密碼。");
                return;
            }
        }

        /// <summary>
        /// 建立Temp資料夾，暫存要處裡的檔案
        /// </summary>
        private string BuildTempFolder()
        {
            // 取得桌面目錄的完整路徑
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            // 定義新的資料夾名稱
            string newFolderName = "Temp";

            // 使用 Path.Combine 方法結合相對路徑和資料夾名稱
            string newFolderPath = System.IO.Path.Combine(desktopPath, newFolderName);

            // 如果資料夾不存在，則建立資料夾
            if (!Directory.Exists(newFolderPath)) Directory.CreateDirectory(newFolderPath);
            //如果存在就刪除資料夾內的所有檔案
            else
            {
                string[] Paths = Directory.GetFiles(newFolderPath);

                // 刪除目錄中的所有檔案
                foreach (string filePath in Paths)
                {
                    File.Delete(filePath);
                }
            }
            return newFolderPath;
        }

        /// <summary>
        /// PDF解密
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="file">檔案</param>
        /// <param name="password">解密密碼</param>
        private void PDFDecrypt(string path, string file, string password)
        {
            // 讀取原始PDF檔案
            using (var reader = new iText.Kernel.Pdf.PdfReader(path + "\\" + file, new iText.Kernel.Pdf.ReaderProperties().SetPassword(Encoding.Default.GetBytes(password))))
            {
                reader.SetUnethicalReading(true);

                // 建立新的PDF檔案
                using (var writer = new iText.Kernel.Pdf.PdfWriter(path + "\\Decrypted-" + file))
                {
                    using (var pdf = new iText.Kernel.Pdf.PdfDocument(reader, writer))
                    {
                        // 關閉PDF文件
                        pdf.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Excel解密
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="file">檔案</param>
        /// <param name="password">解密密碼</param>
        private void ExcelDecrypt(string path, string file, string password)
        {
            // 建立Excel Application和Workbook物件
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = excelApp.Workbooks.Open(path + "\\" + file, Password: password);

            // 解除密碼保護
            workbook.Password = "";

            // 將解除密碼保護後的檔案另存新檔
            workbook.SaveAs(path + "\\Decrypted-" + file, ConflictResolution: XlSaveConflictResolution.xlLocalSessionChanges);

            // 關閉Workbook和Excel Application物件
            workbook.Close();
        }

        /// <summary>
        /// Word解密
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="file">檔案</param>
        /// <param name="password">解密密碼</param>
        private void WordDecrypt(string path, string file, string password)
        {
            // 建立Word Application和Document物件
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            // 開啟Word檔案
            object missing = System.Reflection.Missing.Value;
            object readOnly = false;
            object passwordDocument = password;
            object originalFormat = WdOriginalFormat.wdOriginalDocumentFormat;
            Document document = wordApp.Documents.Open(path + "\\" + file, missing, readOnly, missing, passwordDocument);

            // 將檔案密碼設為 ""
            document.Password = "";
            document.SaveAs2(path + "\\Decrypted-" + file, WdSaveFormat.wdFormatXMLDocument);

            document.Close();
            wordApp.Quit();
        }

        /// <summary>
        /// ZIP壓縮檔解密
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="file">檔案</param>
        /// <param name="password">解密密碼</param>
        /// <param name="processZIP">處理解壓縮檔案的資料匣</param>
        private void ZIPDecrypt(string path, string File, string password, string processZIP)
        {
            string filePath = Path.Combine(path, File);
            //把檔案用密碼解壓縮
            using (var zip = ZipFile.Read(filePath))
            {
                // 設置密碼
                zip.Password = password;

                // 解壓縮到指定的目錄
                zip.ExtractAll(processZIP, ExtractExistingFileAction.OverwriteSilently);
            }
            //把解壓縮完的檔案在壓縮回去，沒有設定密碼
            using (var zip = new ZipFile(Encoding.UTF8))
            {
                var files = Directory.GetFiles(processZIP);

                foreach (var file in files)
                {
                    zip.AddFile(file, string.Empty);
                }

                zip.Save(path + "\\Decrypted-" + File);
            }
            //刪除資料夾，避免後面抓得出問題
            if (Directory.Exists(processZIP)) Directory.Delete(processZIP, true);
        }
    }
}
