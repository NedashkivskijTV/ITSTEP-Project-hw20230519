using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_UdpClient
{
    public partial class FormCreateChatHistoryStartDate : Form
    {

        ChatLibrary.Message message;

        public FormCreateChatHistoryStartDate(ChatLibrary.Message message)
        {
            InitializeComponent();

            this.message = message;
        }

        private void FormCreateChatHistoryStartDate_Load(object sender, EventArgs e)
        {
            dateTimePickerStartDate.Value = message.SendingTime;
        }

        private void btnLoud_Click(object sender, EventArgs e)
        {
            message.SendingTime = dateTimePickerStartDate.Value.Date;

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
