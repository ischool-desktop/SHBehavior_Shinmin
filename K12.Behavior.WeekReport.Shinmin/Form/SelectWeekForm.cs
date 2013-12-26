using System;

namespace K12.Behavior.WeekReport.Shinmin
{
    public partial class SelectWeekForm : SelectDateRangeForm
    {
        private bool _FixToWeek = true;

        public bool FixToWeek
        {
            get { return _FixToWeek; }
            set
            {
                _FixToWeek = value;
                if (!value)
                    errorProvider1.Clear();
            } 
        }

        public SelectWeekForm()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            dateTimeInput2.Enabled = false;
            //���o�Ӷg���P���@
            DateTime weekFirstDay = GetWeekFirstDay(DateTime.Today.AddDays(-7));
            dateTimeInput1.Text = weekFirstDay.ToShortDateString();

            _startDate = weekFirstDay;
            //�P����
            _endDate = weekFirstDay.AddDays(6);
            dateTimeInput1.Text = weekFirstDay.ToShortDateString();
            dateTimeInput2.Text = weekFirstDay.AddDays(6).ToShortDateString();
            _printable = true;
        }

        //�ǤJ�@��-7�Ѫ����
        private DateTime GetWeekFirstDay(DateTime inputDate)
        {
            DateTime firstDay;
            double day = 0;

            //�p�G�o�Ӥ���O
            switch (inputDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    break;
                case DayOfWeek.Tuesday:
                    day = -1;
                    break;
                case DayOfWeek.Wednesday:
                    day = -2;
                    break;
                case DayOfWeek.Thursday :
                    day = -3;
                    break;
                case DayOfWeek.Friday :
                    day = -4;
                    break;
                case DayOfWeek.Saturday :
                    day = -5;
                    break;
                case DayOfWeek.Sunday :
                    day = -6;
                    break;
            }

            inputDate = inputDate.AddDays(day);
            firstDay = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day);
            return firstDay;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_printable)
                dateTimeInput1.Text = _startDate.ToShortDateString();
            timer1.Stop();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dateTimeInput1_TextChanged(object sender, EventArgs e)
        {
            if (!DesignMode && _FixToWeek)
            {
                //if (Validate(dateTimeInput1.Value.ToShortDateString()))
                //{
                    _startDate = GetWeekFirstDay(DateTime.Parse(dateTimeInput1.Value.ToShortDateString()));
                    _endDate = _startDate.AddDays(6);
                    _printable = true;
                    dateTimeInput1.Text = _startDate.ToShortDateString();
                    dateTimeInput2.Text = _endDate.ToShortDateString();
                //}
                //else
                //{
                //    _printable = false;
                //}

                if (!_printable)
                {
                    timer1.Stop();
                    errorProvider1.SetError(dateTimeInput1, "��J����榡���~");
                }
                else
                {
                    if (dateTimeInput1.Text != _startDate.ToShortDateString() && timer1 != null)
                        timer1.Start();
                    errorProvider1.Clear();
                }
            }    
        }
    }
}