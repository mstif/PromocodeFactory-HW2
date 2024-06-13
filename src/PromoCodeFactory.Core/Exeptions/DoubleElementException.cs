using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Exeptions
{
    public class DoubleElementException: Exception
    {
        public DoubleElementException() : base("Этот элемент уже существует!") { }
    }
}
