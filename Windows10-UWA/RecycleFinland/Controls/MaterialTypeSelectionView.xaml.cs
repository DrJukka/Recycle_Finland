using RecycleFinland.Engine;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RecycleFinland.Controls
{
    public delegate void MaterialTypeSelected(int type);

    public sealed partial class MaterialTypeSelectionView : UserControl
    {
        public CloseView CloseView;
        public MaterialTypeSelected MaterialTypeSelected;
        private int _selectedType = 0; // 0 means All

        public int SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                _selectedType = value;
            }
        }

        public MaterialTypeSelectionView()
        {
            this.InitializeComponent();

            var items = new List<MaterialTypeModel>();

            foreach (KeyValuePair<int, string> entry in JLYConstants.materialTypes)
            {
                items.Add(new MaterialTypeModel(entry.Value, entry.Key));
            }

            MaterialTypeSelectionListView.ItemsSource = items;
            MaterialTypeSelectionListView.SelectedIndex = 0;
        }

        public void Show()
        {
            List<MaterialTypeModel> array = (List<MaterialTypeModel>)MaterialTypeSelectionListView.ItemsSource;

            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Code == _selectedType)
                {
                    System.Diagnostics.Debug.WriteLine("Fond " + _selectedType + " == " + array[i].Name);
                    MaterialTypeSelectionListView.SelectedIndex = i;
                    break;
                }
            }
        }
        private void backFromTypesList_Click(object sender, RoutedEventArgs e)
        {
            CloseView?.Invoke();
        }
        private void MaterialTypeSelectionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MaterialTypeModel SelectedItem = (MaterialTypeModel)e.ClickedItem;

            if ((_selectedType != SelectedItem.Code))
            {
                _selectedType = SelectedItem.Code;
                MaterialTypeSelected?.Invoke(_selectedType);
                return;
            }
        }
    }
}
