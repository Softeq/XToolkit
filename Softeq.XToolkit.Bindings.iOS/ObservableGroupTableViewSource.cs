// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Foundation;
using Softeq.XToolkit.Common;
using Softeq.XToolkit.Common.Collections;
using UIKit;

namespace Softeq.XToolkit.Bindings.iOS
{
    public class ObservableGroupTableViewSource<T> : UITableViewSource
    {
        private readonly Func<UITableView, NSIndexPath, IList<T>, UITableViewCell> _getCellViewFunc;
        private readonly Func<IList<T>, nint, nfloat> _getFooterHeightFunc;
        private readonly Func<UITableView, IList<T>, nint, UIView> _getFooterViewFunc;
        private readonly Func<IList<T>, nint, nfloat> _getHeaderHeightFunc;
        private readonly Func<UITableView, nint, IList<T>, UIView> _getHeaderViewFunc;
        private readonly Func<IList<T>, nint, nint> _getRowInSectionCountFunc;
        private readonly Func<IList<T>, nint> _getSectionCountFunc;
        private readonly Action<T, NSIndexPath> _rowSelectedAction;
        private readonly WeakReferenceEx<UITableView> _tableView;

        public ObservableGroupTableViewSource(
            ObservableRangeCollection<T> dataSource,
            UITableView tableView,
            Func<UITableView, NSIndexPath, IList<T>, UITableViewCell> getCellViewFunc,
            Func<IList<T>, nint> getSectionCountFunc,
            Func<IList<T>, nint, nint> getRowInSectionCountFunc,
            Func<UITableView, nint, IList<T>, UIView> getHeaderViewFunc = null,
            Func<IList<T>, nint, nfloat> getHeaderHeightFunc = null,
            Func<UITableView, IList<T>, nint, UIView> getFooterViewFunc = null,
            Func<IList<T>, nint, nfloat> getFooterHeightFunc = null,
            Action<T, NSIndexPath> rowSelectedAction = null)
        {
            DataSource = dataSource;
            _tableView = WeakReferenceEx.Create(tableView);
            _getCellViewFunc = getCellViewFunc;
            _getHeaderViewFunc = getHeaderViewFunc;
            _getHeaderHeightFunc = getHeaderHeightFunc;
            _getSectionCountFunc = getSectionCountFunc;
            _getRowInSectionCountFunc = getRowInSectionCountFunc;
            _getFooterViewFunc = getFooterViewFunc;
            _getFooterHeightFunc = getFooterHeightFunc;
            _rowSelectedAction = rowSelectedAction;

            DataSource = dataSource;
            DataSource.CollectionChanged += OnCollectionChanged;
        }
        
        public ObservableRangeCollection<T> DataSource { get; private set; }

        public nfloat? HeightForRow { get; set; }

        public nfloat? HeightForHeader { get; set; }

        public nfloat? HeightForFooter { get; set; }

        public void ResetCollection(ObservableRangeCollection<T> dataSource)
        {
            if (DataSource != null)
            {
                DataSource.CollectionChanged -= OnCollectionChanged;
            }

            DataSource = dataSource;
            DataSource.CollectionChanged += OnCollectionChanged;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return _getCellViewFunc.Invoke(tableView, indexPath, DataSource);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return _getHeaderViewFunc != null
                ? _getHeaderViewFunc.Invoke(tableView, section, DataSource)
                : new UIView();
        }

        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            return _getFooterViewFunc != null
                ? _getFooterViewFunc.Invoke(tableView, DataSource, section)
                : new UIView();
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _getSectionCountFunc.Invoke(DataSource);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _getRowInSectionCountFunc.Invoke(DataSource, section);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return HeightForRow ?? 0;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (_getHeaderHeightFunc != null)
            {
                return _getHeaderHeightFunc.Invoke(DataSource, section);
            }

            return HeightForHeader ?? 0;
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            if (_getFooterHeightFunc != null)
            {
                return _getFooterHeightFunc.Invoke(DataSource, section);
            }

            return HeightForFooter ?? 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = DataSource[indexPath.Section];
            _rowSelectedAction?.Invoke(item, indexPath);
        }

        private void OnCollectionChanged(object sender, EventArgs e)
        {
            _tableView.Target?.ReloadData();
        }
    }
}