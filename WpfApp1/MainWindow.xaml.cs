using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Entity;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Entities entities;

        private int _pages = 0;

        private int _selectedPage = 0;

        private List<Agent> _agentList;

        private const int maxCountOfLineDg = 10;

        public ObservableCollection<AgentModel> AgentModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            UpdateDB();

            FillCB();
        }

        private void FillCB()
        {
            AgentType[] types = entities.AgentType.ToArray();

            List<string> typeTitles = new List<string>();

            typeTitles.Add("-----");

            foreach (AgentType type in types)
            {
                typeTitles.Add(type.Title);
            }

            cbType.ItemsSource = typeTitles;
        }

        public void UpdateDB()
        {
            entities = new Entities();

            _agentList = entities.Agent.ToList();

            _pages = (int)Math.Ceiling((double)_agentList.Count / maxCountOfLineDg)-1;

            _selectedPage = 0;

            ChangePage(0);
            ChangePageView(0);

            lbCount.Content = $"Всего: {_agentList.Count}";

            SelectAgents();
        }

        private void ReloadDB()
        {
            _pages = (int)Math.Ceiling((double)_agentList.Count / maxCountOfLineDg)-1;

            _selectedPage = 0;

            ChangePage(0);
            ChangePageView(0);

            lbCount.Content = $"Всего: {_agentList.Count}";

            SelectAgents();
        }

        public void UpdateDBPriorUp()
        {
            _agentList = _agentList.OrderBy(_ => _.Priority).ToList();

            _pages = (int)Math.Ceiling((double)_agentList.Count / maxCountOfLineDg) - 1;

            _selectedPage = 0;

            ChangePage(0);
            ChangePageView(0);

            lbCount.Content = $"Всего: {_agentList.Count}";

            SelectAgents();
        }
        public void UpdateDBPriorDwn()
        {
            _agentList = _agentList.OrderByDescending(_ => _.Priority).ToList();

            _pages = (int)Math.Ceiling((double)_agentList.Count / maxCountOfLineDg) - 1;

            _selectedPage = 0;

            ChangePage(0);
            ChangePageView(0);

            lbCount.Content = $"Всего: {_agentList.Count}";

            SelectAgents();
        }

        private void SelectAgents()
        {
            int selectStartIndex = _selectedPage * maxCountOfLineDg;

            int selectEndIndex = selectStartIndex;

            if (selectStartIndex + maxCountOfLineDg <= _agentList.Count)
            {
                selectEndIndex += maxCountOfLineDg;
            }
            else
            {
                selectEndIndex = _agentList.Count - 1;
            }

            if (AgentModel == null)
            {
                AgentModel = new ObservableCollection<AgentModel>();
            }
            else
            {
                AgentModel.Clear();
            }

            for (int i = selectStartIndex;i < selectEndIndex;i++)
            {
                AgentModel model = new AgentModel();

                model.Name = _agentList[i].DirectorName;

                AgentType[] agentTypes = entities.AgentType.ToArray();

                model.Type = agentTypes.FirstOrDefault(_ =>_.ID == _agentList[i].AgentTypeID).Title;
                model.Phone = _agentList[i].Phone;
                model.Priority = _agentList[i].Priority;
                model.ID = _agentList[i].ID;

                AgentModel.Add(model);
            }
        }

        private void btnNavThree_Click(object sender, RoutedEventArgs e)
        {
            int page = Convert.ToInt32((sender as Button).Content);

            ChangePage(page);

            ChangePageView(page);
            
        }

        private void btnNavTwo_Click(object sender, RoutedEventArgs e)
        {
            int page = Convert.ToInt32((sender as Button).Content);

            ChangePage(page);

            ChangePageView(page);
        }

        private void btnNavOne_Click(object sender, RoutedEventArgs e)
        {
            int page = Convert.ToInt32((sender as Button).Content);

            ChangePage(page);

            ChangePageView(page);
        }

        private void ChangePageView(int page)
        {
            if(page == _pages)
            {
                btnNavThree.Content = page;
                btnNavTwo.Content = page-1;
                btnNavOne.Content = page-2;
            }
            else if(page == 0)
            {
                btnNavThree.Content = page + 2;
                btnNavTwo.Content = page + 1;
                btnNavOne.Content = page;
            }
            else
            {
                btnNavThree.Content = page + 1;
                btnNavTwo.Content = page;
                btnNavOne.Content = page -1;
            }
        }

        private void ChangePage(int page)
        {
            _selectedPage = page;

            SelectAgents();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if(lbAgents.SelectedItem == null)
            {
                return;
            }

            AgentModel agentModel = lbAgents.SelectedItem as AgentModel;


            Agent agent = entities.Agent.First(_ => _.ID == agentModel.ID);

            entities.Agent.Remove(agent);

            entities.SaveChanges();

            UpdateDB();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow(this);

            addWindow.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lbAgents.SelectedItem == null)
            {
                return;
            }

            AgentModel agentModel = lbAgents.SelectedItem as AgentModel;


            Agent agent = entities.Agent.First(_ => _.ID == agentModel.ID);

            EditWindow edit = new EditWindow(agent, this);

            edit.ShowDialog();
        }

        private void cbPrior_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sort();
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sort();
        }

        private void Sort()
        {
            UpdateDB();

            if (cbType.SelectedIndex > 0)
            {
                AgentType types = entities.AgentType.First(_ => _.Title == cbType.SelectedValue);

                _agentList = _agentList.Where(_=>_.AgentTypeID == types.ID).ToList();

                ReloadDB();
            }

            if(cbPrior.SelectedIndex == 1)
            {
                UpdateDBPriorDwn();
            }
            else if(cbPrior.SelectedIndex == 2)
            {
                UpdateDBPriorUp();
            }
        }
    }
}
