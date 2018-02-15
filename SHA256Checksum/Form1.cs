using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHA256Checksum
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (this.openFileXML.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                this.txtFileName.Text = this.openFileXML.FileName;
                this.lblChecksum.Text = @"MD5 Checksum for: " + Path.GetFileName(this.openFileXML.FileName);
                this.txtResult.Text = this.CalculateChecksum(this.openFileXML.FileName);
            }
            catch (Exception ex)
            {
                this.txtResult.Text = ex.Message;
            }
        }

        private string CalculateChecksum(string filename)
        {
            using (FileStream fileStream = File.OpenRead(filename))
                return GetSha512Buffered(fileStream);
        }


        private static string GetSha512Buffered(Stream p_streamIn)
        {
            string _result;

            Process.GetCurrentProcess();
            const int _bufferSizeForMd5Hash = 1024 * 1024 * 8;

            using (var _md5Prov = new SHA256Managed())
            {
                int _readCount;
                var _bytesTransfered = 0;
                var _buffer = new byte[_bufferSizeForMd5Hash];

                while ((_readCount = p_streamIn.Read(_buffer, 0, _buffer.Length)) != 0)
                {
                    if (_bytesTransfered + _readCount == p_streamIn.Length)
                    {
                        _md5Prov.TransformFinalBlock(_buffer, 0, _readCount);
                    }
                    else
                    {
                        _md5Prov.TransformBlock(_buffer, 0, _bufferSizeForMd5Hash, _buffer, 0);
                    }
                    _bytesTransfered += _readCount;
                }

                _result = BitConverter.ToString(_md5Prov.Hash).Replace("-", String.Empty).ToLower();
                _md5Prov.Clear();
            }

            return _result;
        }

        private static string GetSha512BufferedStream(Stream p_stream)
        {
            var _bufferedStream = new BufferedStream(p_stream, 1024 * 1024 * 8);
            var _sha256 = new SHA256Managed();

            var _checksum = _sha256.ComputeHash(_bufferedStream);

            var _process = Process.GetCurrentProcess();
            return BitConverter.ToString(_checksum).Replace("-", String.Empty);
        }
    }
}
