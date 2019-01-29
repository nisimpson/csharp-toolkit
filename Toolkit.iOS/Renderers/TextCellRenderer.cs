using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TextCell), typeof(Nerdshoe.iOS.NSTextCellRenderer))]
namespace Nerdshoe.iOS
{
    public class NSTextCellRenderer : TextCellRenderer
    {
        const string DisclosureNone = "none";
        const string Disclosure = "disclosure";
        const string DisclosureDetail = "disclosure-detail";
        const string DisclosureCheckmark = "disclosure-checkmark";
        
        public override UITableViewCell GetCell(Cell item,
            UITableViewCell reusableCell,
            UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            CheckEffects(item, cell);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            switch(item.StyleId) {
                case Disclosure:
                    cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    break;

                case DisclosureDetail:
                    cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
                    break;

                case DisclosureCheckmark:
                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                    break;

                default:
                    cell.Accessory = UITableViewCellAccessory.None;
                    break;
            }

            return cell;
        }

        private void CheckEffects(Cell item, UITableViewCell cell)
        {
           foreach(Effect effect in item.Effects) {
                if (effect is IOSLongPressEffect longPress) {
                    longPress.AttachCell(item, cell);
                }
            }
        }
    }
}
