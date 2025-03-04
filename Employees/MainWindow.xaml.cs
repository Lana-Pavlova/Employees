using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using static Employees.MainWindow;

namespace Employees
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Employee> employees = new ObservableCollection<Employee>();
        private List<Employee> allEmployees;
        public MainWindow()
        {
            InitializeComponent();
            InitializeComboBoxes();
            dgEmployees.ItemsSource = employees;
        }
        private void InitializeComboBoxes()
        {
            cmbEducation.Items.Add("Начальное");
            cmbEducation.Items.Add("Среднее");
            cmbEducation.Items.Add("Средне-спеиальное");
            cmbEducation.Items.Add("Высшее");
            cmbEducation.SelectedIndex = 0;

            cmbBonus.Items.Add("3%");
            cmbBonus.Items.Add("5%");
            cmbBonus.Items.Add("10%");
            cmbBonus.Items.Add("15%");
            cmbBonus.SelectedIndex = 0;

            cmbPosition.Items.Add("Разработчик");
            cmbPosition.Items.Add("Тестировщик");
            cmbPosition.Items.Add("Дизайнер");
            cmbPosition.Items.Add("Менеджер");
            cmbPosition.SelectedIndex = 0;

            cmbFilterEducation.Items.Add("Все");
            cmbFilterEducation.Items.Add("Начальное");
            cmbFilterEducation.Items.Add("Среднее");
            cmbFilterEducation.Items.Add("Средне-спеиальное");
            cmbFilterEducation.Items.Add("Высшее");
            cmbFilterEducation.SelectedIndex = 0;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите нормально имя");
                    return;
                }
                if (datePickerBirthdate.SelectedDate == null)
                {
                    MessageBox.Show("У вас чтоли др нет?");
                    return;
                }
                if (!decimal.TryParse(txtSalary.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal salary))
                {
                    MessageBox.Show("Введи зп нормально");
                    return;
                }


                //создание сотрудника
                Employee newEmployee = new Employee
                {
                    Name = txtName.Text,
                    Birthdate = datePickerBirthdate.SelectedDate.Value,
                    Gender = radioMale.IsChecked == true ? "Мужской" : "Женский",
                    Education = cmbEducation.SelectedItem.ToString(),
                    Position = cmbPosition.SelectedItem.ToString(),
                    Salary = salary,
                    Bonus = cmbBonus.SelectedItem.ToString()
                };

                employees.Add(newEmployee);
                UpdateEmployeeList();
                allEmployees = new List<Employee>(employees);

                txtName.Text = "";
                datePickerBirthdate.SelectedDate = null;
                txtSalary.Text = "";
                cmbEducation.SelectedIndex = 0;
                cmbPosition.SelectedIndex = 0;
                cmbBonus.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem != null)
            {
                Employee employeeToRemove = (Employee)dgEmployees.SelectedItem;
                employees.Remove(employeeToRemove);
            }
            else
            {
                MessageBox.Show("Выберите кого убрать");

            }
            UpdateEmployeeList();
        }
        private void UpdateEmployeeList()
        {
            dgEmployees.Items.Refresh();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgEmployees.ItemsSource = employees;
            }
            else
            {
                var filteredEmployees = employees.Where(emp => emp.Name.ToLower().Contains(searchText)).ToList();
                dgEmployees.ItemsSource = filteredEmployees;
            }
        }

        private void CmbFilterEducation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedEducation = cmbFilterEducation.SelectedItem?.ToString();

            //Если список allEmployeees пуст, значит он еще не был инициализирован
            if (allEmployees == null || !allEmployees.Any())
            {
                allEmployees = new List<Employee>(employees); //Копируем ObservableCollection в List
            }
            if (selectedEducation == "Все" || string.IsNullOrEmpty(selectedEducation))
            {
                dgEmployees.ItemsSource = allEmployees;
            }
            else
            {
                var filteredEmployees = allEmployees.Where(emp => emp.Education == selectedEducation).ToList();
                dgEmployees.ItemsSource = filteredEmployees;

            }
        }



        public class Employee
        {
            public string Name { get; set; }
            public DateTime Birthdate { get; set; }
            public string Gender { get; set; }
            public string Education { get; set; }
            public string Position { get; set; }
            public decimal Salary { get; set; }
            public string Bonus { get; set; }

            public int YearsToRetirement
            {
                get
                {
                    int retirementAge = Gender == "Мужской" ? 65 : 60;
                    int age = DateTime.Now.Year - Birthdate.Year;

                    if ((DateTime.Now.DayOfYear < Birthdate.DayOfYear))
                    {
                        age = age - 1;
                    }
                    return retirementAge - age;
                }
            }
        }
    }
}
