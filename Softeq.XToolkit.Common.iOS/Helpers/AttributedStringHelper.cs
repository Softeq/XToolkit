// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Foundation;
using UIKit;

namespace Softeq.XToolkit.Common.iOS.Helpers
{
    public static class AttributedStringHelper
    {
        public static NSMutableAttributedString BuildAttributedString(this string inputString)
        {
            return new NSMutableAttributedString(inputString);
        }

        public static NSMutableAttributedString Font(this NSMutableAttributedString self, UIFont font)
        {
            self.AddAttribute(UIStringAttributeKey.Font, font, new NSRange(0, self.Length));
            return self;
        }

        public static NSMutableAttributedString Foreground(this NSMutableAttributedString self, UIColor color,
            NSRange? range = null)
        {
            self.AddAttribute(UIStringAttributeKey.ForegroundColor, color, range ?? new NSRange(0, self.Length));
            return self;
        }

        public static NSMutableAttributedString HighlightStrings(
            this NSMutableAttributedString self, IEnumerable<NSRange> ranges, UIColor color)
        {
            foreach (var r in ranges)
            {
                self.Foreground(color, r);
            }

            return self;
        }

        public static NSMutableAttributedString DetectLinks(this NSMutableAttributedString self, UIColor color,
            NSUnderlineStyle style, out string[] result)
        {
            result = default(string[]);

            var linkPattern = @"([a-z]+://)?([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?";
            var links = Regex.Matches(self.Value, linkPattern);
            if (links.Count == 0)
            {
                return self;
            }

            var linkNames = new List<string>(links.Count);
            for (var i = 0; i < links.Count; i++)
            {
                var item = links[i];
                var range = new NSRange(item.Index, item.Value.Length);
                var linkName = $"link{i}";

                var link = item.Value;
                link = link.Contains("://") ? link : $"http://{link}";

                var url = link.ToNSUrl();

                self.AddLink(url, linkName, color, style, range);
                linkNames.Add(linkName);
            }

            result = linkNames.ToArray();

            return self;
        }

        public static NSMutableAttributedString AddLink(this NSMutableAttributedString self, NSUrl url,
            string linkName, UIColor color, NSUnderlineStyle style, NSRange range)
        {
            self.AddAttribute(new NSString(linkName), url, range);
            self.AddAttribute(UIStringAttributeKey.UnderlineStyle, NSNumber.FromInt32((int)style), range);
            self.AddAttribute(UIStringAttributeKey.UnderlineColor, color, range);
            return self;
        }

        public static NSUrl ToNSUrl(this string link)
        {
            var uri = new System.Uri(link);
            var url = NSUrl.FromString(uri.AbsoluteUri);
            return url;
        }
    }
}