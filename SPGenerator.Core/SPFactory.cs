using SPGenerator.Common;

namespace SPGenerator.Core
{
    public abstract class SPFactory
    {
        public static BaseSPGenerator GetSpGeneratorObject(string nodeText)
        {
            BaseSPGenerator spGeneraror = null;
            switch (nodeText)
            {
                case Constants.insertTreeNodeText:
                    spGeneraror = new InsertSPGenerator();
                    break;
                case Constants.updateTreeNodeText:
                    spGeneraror = new UpdateSPGenerator();
                    break;
                case Constants.getTreeNodeText:
                    spGeneraror = new GetSPGenerator();
                    break;
                case Constants.modelSpTreeNodeText:
                    spGeneraror = new ModelSPGenerator();
                    break;
                case Constants.listTreeNodeText:
                    spGeneraror = new ListSPGenerator();
                    break;
            }
            return spGeneraror;
        }
    }
}
