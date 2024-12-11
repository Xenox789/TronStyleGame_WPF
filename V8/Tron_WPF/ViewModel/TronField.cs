using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tron_WPF.ViewModel
{
    public class TronField : ViewModelBase
    {
        private int _value;
        
        public int CellValue { get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public int X { get; set; }
        public int Y { get; set; }


        

    }
}
