using System;
using System.Runtime.InteropServices;

namespace MethodBehavior
{
    [ComVisible(true)]
    [Guid("ED1F4101-C1A6-4943-94AC-C6AAD0B4D2BB")]
    public interface IComClassInterface
    {
        /// <summary>
        /// EAコネクトイベント
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <returns>アドインの種類</returns>
        [DispId(1)]
        string EA_OnPostInitialized(EA.Repository Repository);

        /// <summary>
        /// メニューアイテムうの取得
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <param name="MenuLocation">メニューが呼ばれた位置</param>
        /// <param name="MenuName">親メニューの項目名</param>
        /// <returns>メニューの項目名</returns>
        [DispId(2)]
        object EA_GetMenuItems(EA.Repository Repository, string MenuLocation, string MenuName);

        /// <summary>
        /// メニュー項目のステータス取得
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <param name="MenuLocation">メニューが呼ばれた位置</param>
        /// <param name="MenuName">親メニューの項目名</param>
        /// <param name="ItemName">メニューの項目名</param>
        /// <param name="IsEnabled">選択可否</param>
        /// <param name="IsChecked">チェック表示</param>
        [DispId(3)]
        void EA_GetMenuState(EA.Repository Repository, string MenuLocation, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked);

        /// <summary>
        /// メニュー項目のクリックイベント
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <param name="MenuLocation">メニューが呼ばれた位置</param>
        /// <param name="MenuName">親メニューの項目名</param>
        /// <param name="ItemName">メニューの項目名</param>
        [DispId(4)]
        void EA_MenuClick(EA.Repository Repository, string MenuLocation, string MenuName, string ItemName);

        /// <summary>
        /// コンテキストのカーソルチェンジイベント
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        /// <param name="GUID">項目のGUID</param>
        /// <param name="ot">項目の種類</param>
        [DispId(5)]
        void EA_OnContextItemChanged(EA.Repository Repository, string GUID, EA.ObjectType ot);

        /// <summary>
        /// プロジェクトの閉じるイベント
        /// </summary>
        /// <param name="Repository">EAリポジトリ</param>
        [DispId(6)]
        void EA_FileClose(EA.Repository Repository);
    }
}
