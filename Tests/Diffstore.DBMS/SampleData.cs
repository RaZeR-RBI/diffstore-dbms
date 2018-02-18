namespace Tests.Diffstore.DBMS
{
    public class SampleData
    {
        public string StringField;        

        public SampleData() { }
        public SampleData(string data) => StringField = data;

        public override bool Equals(object obj) => Equals(obj as SampleData);

        private bool Equals(SampleData other)
        {
            if (other == null) return false;
            return StringField == other.StringField;
        }

        public override int GetHashCode() => StringField.GetHashCode();
    }
}