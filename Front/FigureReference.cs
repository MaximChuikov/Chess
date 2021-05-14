using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLogic;

namespace ChessWPF
{
    public class FigureReference
    {
        public FigureReference(PawnReplacedFigure figure)
        {
            replacedFigure = figure;
        }
        public PawnReplacedFigure replacedFigure;
    }
}
