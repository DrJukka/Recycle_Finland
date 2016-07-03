using RecycleFinland.Engine;
using System;
using System.Collections.Generic;

namespace RecycleFinland
{
    class MaterialTypeModel
    {
        public string Name { private set; get; }
        public int Code { private set; get; }

        private MaterialTypeModel(string name, int code)
        {
            Name = name;
            Code = code;
        }

        static public MaterialTypeModel FromCode(int code)
        {
            string typeString;
            if (JLYConstants.materialTypes.TryGetValue(code, out typeString))
            {
                return new MaterialTypeModel(typeString, code);
            }

            return null;
        }

        static MaterialTypeModel()
        {
            List<MaterialTypeModel> all = new List<MaterialTypeModel>();

            foreach (KeyValuePair<int, string> entry in JLYConstants.materialTypes)
            {
                all.Add(new MaterialTypeModel(entry.Value, entry.Key));
            }

            all.TrimExcess();
            All = all;
        }

        public static IEnumerable<MaterialTypeModel> All { private set; get; }
    }
}
