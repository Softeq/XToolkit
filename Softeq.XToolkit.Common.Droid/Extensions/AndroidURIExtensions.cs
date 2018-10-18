using System;
using Android.App;
using Android.Provider;

namespace Softeq.XToolkit.Common.Droid.Extensions
{
    public static class AndroidURIExtensions
    {
        public static string GetRealPathFromURI(this Android.Net.Uri uri, Activity activity)
        {
            string docId = string.Empty;

            using (var c1 = activity.ContentResolver.Query(uri, null, null, null, null))
            {
                c1.MoveToFirst();
                string documentId = c1.GetString(0);
                docId = documentId.Substring(documentId.LastIndexOf(":", StringComparison.Ordinal) + 1);
            }

            string path = null;

            string selection = MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
            using (var cursor = activity.ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { docId }, null))
            {
                if (cursor == null) return path;
                var columnIndex = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                path = cursor.GetString(columnIndex);
            }

            return path;
        }
    }
}
