using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            try
            {
                string[] wmiNamespaces = {
                    "root\\WMI",
                    "root\\CIMV2",
                    "root\\CIMV2\\ms_409"
                };

                bool dataFound = false;

                foreach (string wmiNamespace in wmiNamespaces)
                {
                    try
                    {
                        ManagementObjectSearcher MOS = new ManagementObjectSearcher(
                            wmiNamespace,
                            "SELECT * FROM MSAcpi_ThermalZoneTemperature"
                        );

                        foreach (ManagementObject Mo in MOS.Get())
                        {
                            if (Mo["CurrentTemperature"] != null)
                            {
                                double currentTemp = Convert.ToDouble(Mo["CurrentTemperature"]);
                                double CPUtemp = (currentTemp / 10.0) - 273.15;
                                listBox1.Items.Add($"CPU ({wmiNamespace}): {CPUtemp:F1} °C");
                                dataFound = true;
                            }
                        }
                    }
                    catch (ManagementException)
                    {
                        continue;
                    }
                }

                if (!dataFound)
                {
                    try
                    {
                        ManagementObjectSearcher altSearcher = new ManagementObjectSearcher(
                            "root\\CIMV2",
                            "SELECT * FROM Win32_TemperatureProbe"
                        );

                        foreach (ManagementObject mo in altSearcher.Get())
                        {
                            if (mo["CurrentReading"] != null)
                            {
                                double temp = Convert.ToDouble(mo["CurrentReading"]);
                                listBox1.Items.Add($"CPU (Alt): {temp:F1} °C");
                                dataFound = true;
                            }
                        }
                    }
                    catch { }
                }

                if (!dataFound)
                {
                    listBox1.Items.Add("Данные о температуре недоступны");
                    MessageBox.Show("Не удалось получить данные о температуре. " +
                                  "Возможно, ваш компьютер не поддерживает этот WMI-провайдер.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения данных: " + ex.Message);
            }
        }
    }
}