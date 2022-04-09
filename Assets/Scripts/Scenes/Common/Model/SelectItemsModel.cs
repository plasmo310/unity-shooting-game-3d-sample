
namespace Scenes.Common.Model
{
    /// <summary>
    /// 複数アイテム選択用Model
    /// </summary>
    public class SelectItemsModel<T>
    {
        /// <summary>
        /// 選択アイテム情報
        /// </summary>
        private readonly T[] _selectItemArray;
        private int _selectItemIndex;

        public SelectItemsModel(T[] selectArray)
        {
            _selectItemArray = selectArray;
            _selectItemIndex = 0;
        }
        
        /// <summary>
        /// 選択しているアイテムを返却する
        /// </summary>
        /// <returns></returns>
        public T GetSelectItem()
        {
            return _selectItemArray[_selectItemIndex];
        }

        /// <summary>
        /// 選択したアイテムのindexを設定
        /// </summary>
        /// <param name="index"></param>
        public void SetSelectIndex(int index)
        {
            _selectItemIndex = index;
        }
        
        /// <summary>
        /// 選択されたアイテムのindexを返却
        /// </summary>
        /// <returns></returns>
        public int GetSelectIndex()
        {
            return _selectItemIndex;
        }

        /// <summary>
        /// 次のindexを選択する
        /// </summary>
        public void ChangeNextSelectIndex()
        {
            _selectItemIndex++;
            if (_selectItemIndex > _selectItemArray.Length - 1)
            {
                _selectItemIndex = 0;
            }
        }

        /// <summary>
        /// 前のindexを選択する
        /// </summary>
        public void ChangePrevItemIndex()
        {
            _selectItemIndex--;
            if (_selectItemIndex < 0)
            {
                _selectItemIndex = _selectItemArray.Length - 1;
            }
        }
    }
}
