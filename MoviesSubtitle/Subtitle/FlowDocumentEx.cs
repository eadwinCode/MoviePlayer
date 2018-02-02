using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace OutlineText
{
    public static class FlowDocumentEx
    {
        public static ICollection<Inline> GetInlines(FlowDocument doc)
        {
            return GetInlines(doc.Blocks);
        }

        public static ICollection<Inline> GetInlines(TextElementCollection<Block> blocks)
        {
            var inlines = new List<Inline>();

            foreach (var block in blocks)
            {
                if (block is Paragraph)
                {
                    inlines.AddRange(((Paragraph)block).Inlines);
                }
                else if (block is Section)
                {
                    inlines.AddRange(GetInlines(((Section)block).Blocks));
                }
            }

            return inlines;
        }
    }
}
