using MethodBehavior.view;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MethodBehavior.control
{
    /// <summary>
    /// 振舞い編集アドイン
    /// </summary>
    public partial class BehaverControl : UserControl
    {
        #region "定数"
        /// <summary>
        /// リーチテキストのマージン
        /// </summary>
        private const int MARGIN_SIZE = 3;

        /// <summary>
        /// 保存しないエラーメッセージ
        /// </summary>
        private const string MSG_NO_SAVED = "編集中の振舞いは保存されませんでした。\r\n"
                                          + "振舞いを保存しますか？";

        /// <summary>
        /// 変更なしのインフォメッセージ
        /// </summary>
        private const string MSG_NO_CHANGED = "振舞いの変更はありません。";
        #endregion

        #region "変数"
        /// <summary>
        /// EAリポジトリ
        /// </summary>
        public EA.Repository repository { private get; set; }

        /// <summary>
        /// 操作オブジェクト
        /// </summary>
        private EA.Method method { get; set; }
        #endregion

        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaverControl()
        {
            InitializeComponent();
        }
        #endregion

        #region "コントローラーのリサーズ"
        /// <summary>
        /// コントローラーのリサーズ
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BehaverControl_Resize(object sender, EventArgs e)
        {
            var totalSize = this.Size;
            var toolStripSize = toolStrip.Size;
            var statusStripSize = statusStrip.Size;

            var width = totalSize.Width - MARGIN_SIZE * 2;
            var hight = totalSize.Height - toolStripSize.Height - statusStripSize.Height - MARGIN_SIZE * 2;

            richTextBox.Size = new Size(width, hight);
            richTextBox.Location = new Point(MARGIN_SIZE, toolStripSize.Height + MARGIN_SIZE);
        }
        #endregion

        #region "操作オブジェクトの更新"
        /// <summary>
        /// 操作オブジェクトの更新
        /// </summary>
        /// <param name="method">操作オブジェクト</param>
        /// <param name="force">強制更新Flg</param>
        public void UpdateMethod(EA.Method method, bool force = false)
        {
            // 変更なし
            if (this.method != null && this.method.MethodGUID == method.MethodGUID) return;

            // 強制更新
            if (!force) force = enableAutoReflesh.Checked;

            // 操作オブジェクトの更新処理
            if (force)
            {
                var isChanged = false;
                var behaviorText = richTextBox.Text.Replace("\n", "\r\n");
                if (this.method != null && behaviorText != this.method.Behavior)
                {
                    var assemblyTitle = (AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), true).Single();
                    var result = MessageBox.Show(MSG_NO_SAVED, assemblyTitle.Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // 保存して、更新する
                        this.method.Behavior = behaviorText;
                        this.method.Update();
                    }
                    else if (result == DialogResult.No)
                    {
                        // 放棄して、更新する
                    }
                    else
                    {
                        // キャンセルして、自動更新を停止する
                        enableAutoReflesh.Checked = false;
                        isChanged = true;
                    }
                }

                if (!isChanged)
                {
                    this.method = method;
                    toolStripStatus.Text = "操作:" + method.Name;

                    richTextBox.ReadOnly = false;
                    richTextBox.Text = method.Behavior;
                }
            }
        }
        #endregion

        #region "操作とのリンクの解除"
        /// <summary>
        /// 操作とのリンクの解除
        /// </summary>
        public void DisconnectMethod()
        {
            if (this.method == null) return;

            var behaviorText = richTextBox.Text.Replace("\n", "\r\n");
            if (behaviorText != this.method.Behavior)
            {
                var assemblyTitle = (AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), true).Single();
                var result = MessageBox.Show(MSG_NO_SAVED, assemblyTitle.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // 保存して、更新する
                    method.Behavior = behaviorText;
                    method.Update();
                }

            }

            method = null;
            enableAutoReflesh.Checked = false;
            richTextBox.ReadOnly = true;

            toolStripStatus.Text = "操作とのリンスはありません。";
        }
        #endregion

        #region "自動更新ボタンのイベント"
        /// <summary>
        /// 自動更新ボタンのイベント
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void enableAutoReflesh_Click(object sender, EventArgs e)
        {
            enableAutoReflesh.Checked = !enableAutoReflesh.Checked;
        }
        #endregion

        #region "振舞いの保存"
        /// <summary>
        /// 振舞いの保存
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void saveBehaver_Click(object sender, EventArgs e)
        {
            if (method == null) return;

            method.Behavior = richTextBox.Text.Replace("\n", "\r\n");
            method.Update();
        }
        #endregion

        #region "ツールステータスのイベント"
        /// <summary>
        /// ツールステータスのイベント
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void toolStripStatus_DoubleClick(object sender, EventArgs e)
        {
            if (method == null) return;
            if (repository == null) return;

            repository.ShowInProjectView(method);
        }
        #endregion

        #region "差分比較"
        /// <summary>
        /// 差分の表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewChange_Click(object sender, EventArgs e)
        {
            if (method == null) return;
            var newBehavior = richTextBox.Text.Replace("\n", "\r\n");

            if (newBehavior == method.Behavior)
            {
                // 変更なし
                var assemblyTitle = (AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), true).Single();
                MessageBox.Show(MSG_NO_CHANGED, assemblyTitle.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // 差分一覧
                var diffChanges = new DiffChangesView(method.Behavior, newBehavior);
                diffChanges.ShowDialog();
            }
        }
        #endregion
    }
}
