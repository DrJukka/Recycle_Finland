using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace RecycleFinland.Controls
{
    public class MaterialTypeModel
    {
        private String _name;
        private int _code;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Code
        {
            get
            {
                return _code;
            }
        }

        public MaterialTypeModel(string name, int code)
        {
            _name = name;
            _code = code;
        }
    }
}
