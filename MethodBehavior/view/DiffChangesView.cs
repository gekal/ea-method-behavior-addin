using System.Windows.Forms;

namespace MethodBehavior.view
{
    /// <summary>
    /// 振舞いの比較画面
    /// </summary>
    public partial class DiffChangesView : Form
    {
        #region "定数"
        /// <summary>
        /// リーチテキストのマージン
        /// </summary>
        private const int MARGIN_SIZE = 3;
        #endregion

        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="leftText">修正前</param>
        /// <param name="rightText">修正後</param>
        public DiffChangesView(string leftText, string rightText)
        {
            InitializeComponent();
            diffControl.LeftText = leftText;
            diffControl.RightText = rightText;
        }
        #endregion

        #region "フォールのリサイズ"
        /// <summary>
        /// フォールのリサイズイベント
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void DiffChanges_Resize(object sender, System.EventArgs e)
        {
            elementHost.Width = this.Width - MARGIN_SIZE * 2 - 10;
            elementHost.Height = this.Height - MARGIN_SIZE * 2 - 34;
        }
        #endregion
    }
}
