using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatLibrary;
using Microsoft.EntityFrameworkCore;

namespace Server_UdpClient
{
    public partial class FormCreateOrEditBlackList : Form
    {

        BlackList blackList;

        List<User> usersAll;
        List<User> usersInBlackList;
        List<User> usersNotInBlackList;
        List<User> usersBlackListCreators;
        List<User> usersNotBlackListCreators;

        public FormCreateOrEditBlackList(BlackList blackList)
        {
            InitializeComponent();

            this.blackList = blackList;

            usersAll = new List<User>();
            usersInBlackList = new List<User>();
            usersNotInBlackList = new List<User>();
            usersBlackListCreators = new List<User>();
            usersNotBlackListCreators = new List<User>();

            // Налаштування відображення інф у елементах dgv - одним суцільним рядком
            dgvUsersBlackListMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsersNotBlackListMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void FormCreateOrEditBlackList_Load(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                usersAll = context.Users.ToList();

                usersInBlackList = context.BlackLists.Where(l => l.CreatorId == blackList.CreatorId).Select(b => b.BlackUser).ToList();

                usersNotInBlackList = usersAll.Where(u => !usersInBlackList.Contains(u)).ToList();
                // Групування по id та вібірка у групах першого лемента для уникнення повторів у колекції
                usersBlackListCreators = context.BlackLists.Select(b => b.Creator).GroupBy(x => x.Id).Select(x => x.First()).ToList();
                usersNotBlackListCreators = usersAll.Where(u => !usersBlackListCreators.Contains(u)).ToList();

                UpdateDgvBlackListMembers();

                cbUserCreator.DataSource = null;
                cbUserCreator.DisplayMember = nameof(User.Login);
                cbUserCreator.ValueMember = nameof(User.Id);

                if (blackList.CreatorId != 0)
                {
                    lbUserCreator.Text = "Users who have already created a blacklist (editing)";

                    cbUserCreator.DataSource = usersBlackListCreators;
                    int index = 0;

                    foreach (User user in usersBlackListCreators)
                    {
                        if (user.Id == blackList.CreatorId)
                        {
                            break;
                        }
                        index++;
                    }
                    cbUserCreator.SelectedIndex = index;
                }
                else
                {
                    lbUserCreator.Text = "Users who have not created a blacklist (creation)";

                    cbUserCreator.DataSource = usersNotBlackListCreators;
                }
            }
        }

        private async void btnOK_Click(object sender, EventArgs e)
        {
            if (cbUserCreator.SelectedIndex != -1 && usersInBlackList.Count > 0)
            {
                await DataUpdate();

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("The fields must be filled, and at least 1 person must be added to the list");
            }
        }

        private async Task DataUpdate()
        {
            blackList.CreatorId = int.Parse("" + cbUserCreator.SelectedValue);

            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                List<User> usersInBlackListBefore = context.BlackLists
                    .Where(x => x.CreatorId == blackList.CreatorId)
                    .Select(b => b.BlackUser)
                    .ToList();

                // Видалення з чорного списка користувачів, що були раніше та видалені під час редагування
                List<User> usersForDeleteFromBlackList = usersInBlackListBefore
                    .Where(u => !(usersInBlackList.Select(x => x.Login).ToList()).Contains(u.Login))
                    .ToList();

                foreach (User user in usersForDeleteFromBlackList)
                {
                    BlackList? blackListForDelete = context.BlackLists
                        .FirstOrDefault(b => b.BlackUserId == user.Id && b.CreatorId == blackList.CreatorId);
                    if (blackListForDelete != null)
                    {
                        context.Entry<BlackList>(blackListForDelete).State = EntityState.Deleted;
                    }
                }
                await context.SaveChangesAsync();

                // Оновлення у чорному списку користувачів, що присутні до та після редагування
                List<User> usersForUpdateInBlackList = usersInBlackListBefore
                    .Where(u => (usersInBlackList.Select(x => x.Login).ToList()).Contains(u.Login))
                    .ToList();

                foreach (User user in usersForUpdateInBlackList)
                {
                    BlackList? blackListForUpdate = context.BlackLists
                        .FirstOrDefault(b => b.BlackUserId == user.Id && b.CreatorId == blackList.CreatorId);
                    if (blackListForUpdate != null)
                    {
                        context.Entry<BlackList>(blackListForUpdate).State = EntityState.Modified;
                    }
                }
                await context.SaveChangesAsync();

                // Додавання до чорного списку користувачів, доданих після редагування (раніше їх не було)
                List<User> usersForAddedToBlackList = 
                    usersInBlackList
                    .Where(u => !(usersInBlackListBefore.Select(x => x.Login).ToList()).Contains(u.Login))
                    .ToList();

                BlackList? blackListForAdded = null;
                foreach (User user in usersForAddedToBlackList)
                {
                    blackListForAdded = context.BlackLists
                        .FirstOrDefault(b => b.BlackUserId == user.Id && b.CreatorId == blackList.CreatorId);
                    if (blackListForAdded == null)
                    {
                        blackListForAdded = new BlackList();
                        blackListForAdded.CreatorId = blackList.CreatorId;
                        blackListForAdded.BlackUserId = user.Id;
                        context.Entry<BlackList>(blackListForAdded).State = EntityState.Added;
                    }
                    blackListForAdded = null;
                }

                await context.SaveChangesAsync();
            }

        }

        private void ShowUsers(List<User> userList)
        {
            MessageBox.Show("userList.Count = " + userList.Count);
            foreach (User user in userList)
            {
                MessageBox.Show("user.Login = " + user.Login);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dgvUsersNotBlackListMembers.SelectedRows.Count > 0)
            {
                int userId = int.Parse(dgvUsersNotBlackListMembers.SelectedRows[0].Cells[0].Value.ToString());
                User user = usersNotInBlackList.Find(u => u.Id == userId);

                usersInBlackList.Add(user);

                usersNotInBlackList.Remove(user);

                UpdateDgvBlackListMembers();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvUsersBlackListMembers.SelectedRows.Count > 0)
            {
                int userId = int.Parse(dgvUsersBlackListMembers.SelectedRows[0].Cells[0].Value.ToString());
                User user = usersInBlackList.Find(u => u.Id == userId);

                usersInBlackList.Remove(user);
                usersNotInBlackList.Add(user);

                UpdateDgvBlackListMembers();
            }
        }

        private void UpdateDgvBlackListMembers()
        {
            dgvUsersBlackListMembers.DataSource = null;
            dgvUsersBlackListMembers.DataSource = usersInBlackList.Select(u => new { UserId = u.Id, Login = u.Login }).ToList();

            dgvUsersNotBlackListMembers.DataSource = null;
            dgvUsersNotBlackListMembers.DataSource = usersNotInBlackList.Select(u => new { UserId = u.Id, Login = u.Login }).ToList();
        }
    }
}
