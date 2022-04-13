using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ACS_WAPConnectionDetails
{
    class MyApplicationContext : ApplicationContext
    {

        private int _formCount;
        private Form1 _form1;

        private Rectangle _form1Position;

        private FileStream _userData;

        private MyApplicationContext()
        {
            _formCount = 0;

            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            try
            {
                // Create a file that the application will store user specific data in.
                _userData = new FileStream(Application.UserAppDataPath + "\\appdata.txt", FileMode.OpenOrCreate);
            }
            catch (IOException e)
            {
                // Inform the user that an error occurred.
                MessageBox.Show("An error occurred while attempting to show the application." +
                                "The error is:" + e.ToString());

                // Exit the current thread instead of showing the windows.
                ExitThread();
            }

            // Create both application forms and handle the Closed event
            // to know when both forms are closed.
            _form1 = new Form1();
            _form1.Closed += new EventHandler(OnFormClosed);
            _form1.Closing += new CancelEventHandler(OnFormClosing);
            _formCount++;

            // Get the form positions based upon the user specific data.
            if (ReadFormDataFromFile())
            {
                // If the data was read from the file, set the form
                // positions manually.
                _form1.StartPosition = FormStartPosition.Manual;

                _form1.Bounds = _form1Position;
            }

            // Show both forms.
            _form1.Show();
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            // When the application is exiting, write the application data to the
            // user file and close it.
            WriteFormDataToFile();

            try
            {
                // Ignore any errors that might occur while closing the file handle.
                _userData.Close();
            }
            catch { }
        }

        private void OnFormClosing(object sender, CancelEventArgs e)
        {
            // When a form is closing, remember the form position so it
            // can be saved in the user data file.
            if (sender is Form1)
                _form1Position = ((Form)sender).Bounds;
        }

        private void OnFormClosed(object sender, EventArgs e)
        {
            // When a form is closed, decrement the count of open forms.

            // When the count gets to 0, exit the app by calling
            // ExitThread().
            _formCount--;
            if (_formCount == 0)
            {
                ExitThread();
            }
        }

        private bool WriteFormDataToFile()
        {
            // Write the form positions to the file.
            UTF8Encoding encoding = new UTF8Encoding();

            RectangleConverter rectConv = new RectangleConverter();
            string form1pos = rectConv.ConvertToString(_form1Position);

            byte[] dataToWrite = encoding.GetBytes("~" + form1pos);

            try
            {
                // Set the write position to the start of the file and write
                _userData.Seek(0, SeekOrigin.Begin);
                _userData.Write(dataToWrite, 0, dataToWrite.Length);
                _userData.Flush();

                _userData.SetLength(dataToWrite.Length);
                return true;
            }
            catch
            {
                // An error occurred while attempting to write, return false.
                return false;
            }
        }

        private bool ReadFormDataFromFile()
        {
            // Read the form positions from the file.
            UTF8Encoding encoding = new UTF8Encoding();
            string data;

            if (_userData.Length != 0)
            {
                byte[] dataToRead = new byte[_userData.Length];

                try
                {
                    // Set the read position to the start of the file and read.
                    _userData.Seek(0, SeekOrigin.Begin);
                    _userData.Read(dataToRead, 0, dataToRead.Length);
                }
                catch (IOException e)
                {
                    string errorInfo = e.ToString();
                    // An error occurred while attempt to read, return false.
                    return false;
                }

                // Parse out the data to get the window rectangles
                data = encoding.GetString(dataToRead);

                try
                {
                    // Convert the string data to rectangles
                    RectangleConverter rectConv = new RectangleConverter();
                    string form1pos = data.Substring(1);// data.Substring(1, data.IndexOf("~", 0) - 1);

                    _form1Position = (Rectangle)rectConv.ConvertFromString(form1pos);

                    return true;
                }
                catch
                {
                    // Error occurred while attempting to convert the rectangle data.
                    // Return false to use default values.
                    return false;
                }
            }
            else
            {
                // No data in the file, return false to use default values.
                return false;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {

            // Create the MyApplicationContext, that derives from ApplicationContext,
            // that manages when the application should exit.

            MyApplicationContext context = new MyApplicationContext();

            // Run the application with the specific context. It will exit when
            // all forms are closed.
            Application.Run(context);
        }
    }
}