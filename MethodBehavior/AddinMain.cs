using MethodBehavior.control;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MethodBehavior
{
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("E34983CA-5F53-4DAE-AB3E-0195377D6F0E")]
    public class AddinMain : IComClassInterface
    {
        #region "メニュー"
        /// <summary>
        /// アドイン名
        /// </summary>
        private string MAIN_ITEM = "-&振舞いアドイン";

        /// <summary>
        /// アドイン名
        /// </summary>
        private string VERSION = "&バージョン";
        #endregion

        #region "定数"
        /// <summary>
        /// アドイン名
        /// </summary>
        private string AddinName = "振舞い";
        #endregion

        #region "変数"
        /// <summary>
        /// アドインの画面
        /// </summary>
        private BehaverControl behaverControl = null;
        #endregion

        #region "EAコネクトイベント"
        /// <summary>
        /// EAコネクトイベント
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <returns>アドインの種類</returns>
        public string EA_OnPostInitialized(EA.Repository Repository)
        {
            if (Repository.LibraryVersion < 1300)
            {

                MessageBox.Show("このアドインの動作には、ビルド1300以降が必要です。");
            }
            else
            {

                behaverControl = Repository.AddWindow(AddinName, typeof(BehaverControl).FullName);
            }

            if (behaverControl != null)
            {
                // 振舞い編集画面の初期化
                behaverControl.repository = Repository;
            }

            return string.Empty;
        }
        #endregion

        #region "メニューアイテムうの取得"
        /// <summary>
        /// メニューアイテムうの取得
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <param name="MenuLocation">メニューが呼ばれた位置</param>
        /// <param name="MenuName">親メニューの項目名</param>
        /// <returns>メニューの項目名</returns>
        public object EA_GetMenuItems(EA.Repository Repository, string MenuLocation, string MenuName)
        {
            if (MenuName == string.Empty)
            {
                return MAIN_ITEM;
            }
            else
            {
                String[] ret = { VERSION };
                return ret;
            }
        }
        #endregion

        #region "メニュー項目のステータス取得"
        /// <summary>
        /// メニュー項目のステータス取得
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <param name="MenuLocation">メニューが呼ばれた位置</param>
        /// <param name="MenuName">親メニューの項目名</param>
        /// <param name="ItemName">メニューの項目名</param>
        /// <param name="IsEnabled">選択可否</param>
        /// <param name="IsChecked">チェック表示</param>
        public void EA_GetMenuState(EA.Repository Repository, string MenuLocation, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            // 何もしない
        }
        #endregion

        #region "メニュー項目のクリックイベント"
        /// <summary>
        /// メニュー項目のクリックイベント
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <param name="MenuLocation">メニューが呼ばれた位置</param>
        /// <param name="MenuName">親メニューの項目名</param>
        /// <param name="ItemName">メニューの項目名</param>
        public void EA_MenuClick(EA.Repository Repository, string MenuLocation, string MenuName, string ItemName)
        {
            if (ItemName == VERSION)
            {
                var assembly = Assembly.GetExecutingAssembly();
                // カスタム属性AssemblyTitleAttributeの取得
                var assemblyTitle = (AssemblyTitleAttribute)assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true).Single();
                MessageBox.Show(assembly.GetName().Version.ToString(), assemblyTitle.Title,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region "コンテキストのカーソルチェンジイベント"
        /// <summary>
        /// コンテキストのカーソルチェンジイベント
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <param name="GUID">項目のGUID</param>
        /// <param name="ot">項目の種類</param>
        public void EA_OnContextItemChanged(EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            if (behaverControl == null || ot != EA.ObjectType.otMethod) return;

            behaverControl.UpdateMethod(Repository.GetMethodByGuid(GUID));
        }
        #endregion

        #region "プロジェクトの閉じるイベント"
        /// <summary>
        /// プロジェクトの閉じるイベント
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        public void EA_FileClose(EA.Repository Repository)
        {
            if (behaverControl == null) return;

            behaverControl.DisconnectMethod();
        }
        #endregion
    }
}
