using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sample
{
    /// <summary>
    /// Simple polynomial class used for the fractal rendering.
    /// </summary>
    public class Polynomial : IEquatable<Polynomial>
    {
        private readonly Dictionary<UInt32, Complex> terms;
        private String repr;

        /// <summary>
        /// Creates a new zero polynomial.
        /// </summary>
        public Polynomial()
        {
            terms = new Dictionary<UInt32, Complex>();
        }

        /// <summary>
        /// Adds a term to this polynomial.
        /// </summary>
        public void AddTerm(UInt32 power, Complex coeff)
        {
            if (!terms.ContainsKey(power))
                terms.Add(power, coeff);
            else
                terms[power] += coeff;
        }

        /// <summary>
        /// Returns the derivative of a polynomial.
        /// </summary>
        public static Polynomial Derivative(Polynomial poly)
        {
            var derv = Polynomial.Zero;

            foreach (var power in poly.terms.Keys.Where(p => p != 0))
                derv.AddTerm(power - 1, poly.terms[power] * power);

            return derv;
        }

        private Complex Pow(Complex z, UInt32 n)
        {
            Complex val = Complex.One;

            for (int t = 0; t < n; ++t)
                val *= z;

            return val;
        }

        /// <summary>
        /// Evaluates the polynomial at a point.
        /// </summary>
        public Complex Evaluate(Complex z)
        {
            Complex val = Complex.Zero;

            foreach (var term in terms)
            {
                if (term.Key == 0)
                    val += term.Value;
                else
                    val += Pow(z, term.Key) * term.Value;
            }

            return val;
        }

        /// <summary>
        /// Returns the roots of a polynomial.
        /// </summary>
        /// <remarks>
        /// Uses probabilistic Newton to locate roots.
        /// </remarks>
        public static Tuple<ISet<Complex>, Complex> Roots(Polynomial poly)
        {
            uint iters = 150, inc = poly.Degree * 5;
            const double threshold = 1e-10;
            const double rate = 0.5;

            if (poly.Degree == 0) {
                return new Tuple<ISet<Complex>, Complex>(new HashSet<Complex>(), Complex.Zero);
            }

            var derv = Polynomial.Derivative(poly);
            var roots = new HashSet<Complex>();
            var random = new Random();
            int count = 0, multiplicity = 1;
            double bound = 1;

            while (poly.Degree > 0)
            {
                var zn = Complex.FromPolarCoordinates(bound * random.NextDouble(),
                                                      random.NextDouble() * Math.PI * 2);

                for (int t = 0; t < iters; ++t)
                {
                    zn -= multiplicity * poly.Evaluate(zn) / derv.Evaluate(zn);
                    if (poly.Evaluate(zn).Magnitude < threshold) break;
                    if (Double.IsInfinity(zn.Magnitude)) break;
                }

                if (poly.Evaluate(zn).Magnitude < threshold)
                {
                    var root = Polynomial.Zero;
                    root.AddTerm(1, Complex.One);
                    root.AddTerm(0, -zn);
                    roots.Add(zn);

                    /* Divide out the root and differentiate */
                    derv = Polynomial.Derivative(poly /= root);

                    multiplicity = 1;
                }
                else
                {
                    bound += rate;
                    ++count;

                    if (count % inc == 0)
                        ++multiplicity;
                }
            }

            return new Tuple<ISet<Complex>, Complex>(roots, poly.terms[0]);
        }

        #region Utilities

        /// <summary>
        /// Gets the zero polynomial.
        /// </summary>
        public static Polynomial Zero { get { return new Polynomial(); } }

        /// <summary>
        /// Gets the unit polynomial.
        /// </summary>
        public static Polynomial One
        {
            get
            {
                var poly = Polynomial.Zero;
                poly.AddTerm(0, Complex.One);
                return poly;
            }
        }

        /// <summary>
        /// Gets the degree of this polynomial.
        /// </summary>
        public UInt32 Degree
        {
            get
            {
                var nonzeroTerms = terms.Keys.Where(p => terms[p] != Complex.Zero);
                return (nonzeroTerms.Count() == 0) ? 0 : nonzeroTerms.Max();
            }
        }

        /// <summary>
        /// Gets the leading term of this polynomial.
        /// </summary>
        public Tuple<UInt32, Complex> LeadingTerm
        {
            get { return new Tuple<UInt32, Complex>(Degree, terms[Degree]); }
        }

        #endregion

        #region Arithmetic

        /// <summary>
        /// Adds two polynomials together.
        /// </summary>
        public static Polynomial Add(Polynomial a, Polynomial b)
        {
            Polynomial sum = Polynomial.Zero;

            foreach (var term in a.terms)
                sum.AddTerm(term.Key, term.Value);

            foreach (var term in b.terms)
                sum.AddTerm(term.Key, term.Value);

            return sum;
        }

        /// <summary>
        /// Subtracts one polynomial from the other.
        /// </summary>
        public static Polynomial Subtract(Polynomial a, Polynomial b)
        {
            Polynomial difference = Polynomial.Zero;

            foreach (var term in a.terms)
                difference.AddTerm(term.Key, term.Value);

            foreach (var term in b.terms)
                difference.AddTerm(term.Key, -term.Value);

            return difference;
        }

        /// <summary>
        /// Multiplies two polynomials together.
        /// </summary>
        public static Polynomial Multiply(Polynomial a, Polynomial b)
        {
            Polynomial product = Polynomial.Zero;

            foreach (var termA in a.terms)
                foreach (var termB in b.terms)
                    product.AddTerm(termB.Key + termA.Key, termB.Value * termA.Value);
                
            return product;
        }

        /// <summary>
        /// Internal division method which produces quotient and remainder.
        /// </summary>
        private static Tuple<Polynomial, Polynomial> DivRem(Polynomial a, Polynomial b)
        {
            if (b.Equals(Polynomial.Zero))
                throw new ArithmeticException("Polynomial division by zero");

            var quotient = Polynomial.Zero;
            var remainder = a;

            while (!remainder.Equals(Polynomial.Zero) && remainder.Degree >= b.Degree)
            {
                UInt32 tPower = remainder.LeadingTerm.Item1 - b.LeadingTerm.Item1;
                Complex tCoeff = remainder.LeadingTerm.Item2 / b.LeadingTerm.Item2;

                Polynomial t = Polynomial.Zero;
                t.AddTerm(tPower, tCoeff);

                remainder -= t * b;
                quotient += t;
            }

            return new Tuple<Polynomial, Polynomial>(quotient, remainder);
        }

        /// <summary>
        /// Divides one polynomial by the other and returns the quotient.
        /// </summary>
        public static Polynomial Divide(Polynomial a, Polynomial b)
        {
            return DivRem(a, b).Item1;
        }

        /// <summary>
        /// Divides one polynomial by the other and returns the remainder.
        /// </summary>
        public static Polynomial Remainder(Polynomial a, Polynomial b)
        {
            return DivRem(a, b).Item2;
        }

        #endregion

        #region Operators

        public static Polynomial operator +(Polynomial a, Polynomial b)
        {
            return Polynomial.Add(a, b);
        }

        public static Polynomial operator -(Polynomial a, Polynomial b)
        {
            return Polynomial.Subtract(a, b);
        }

        public static Polynomial operator *(Polynomial a, Polynomial b)
        {
            return Polynomial.Multiply(a, b);
        }

        public static Polynomial operator /(Polynomial a, Polynomial b)
        {
            return Polynomial.Divide(a, b);
        }

        public static Polynomial operator %(Polynomial a, Polynomial b)
        {
            return Polynomial.Remainder(a, b);
        }

        #endregion

        #region Miscellaneous

        public bool Equals(Polynomial other)
        {
            foreach (var term in other.terms.Where(x => x.Value != Complex.Zero))
            {
                if (!terms.ContainsKey(term.Key))
                    return false;
                if (terms[term.Key] != term.Value)
                    return false;
            }

            foreach (var term in terms.Where(x => x.Value != Complex.Zero))
            {
                if (!other.terms.ContainsKey(term.Key))
                    return false;
                if (other.terms[term.Key] != term.Value)
                    return false;
            }

            return true;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Polynomial))
                return false;
            return Equals((Polynomial)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            foreach (var term in terms.Where(x => x.Value != Complex.Zero))
                hashCode ^= term.GetHashCode();

            return hashCode;
        }

        public override String ToString()
        {
            if (repr != null) {
                return repr;
            }

            return String.Join(" + ", terms.Keys.Where(p => terms[p] != Complex.Zero).OrderByDescending(x => x).Select(power =>
            {
                String exponent;

                if (power == 0) {
                    exponent = "";
                } else {
                    exponent = String.Format("z^{0}", power);
                }

                if (terms[power].Real == 0) {
                    return String.Format("{0}{1}", ParseDouble(terms[power].Imaginary), exponent);
                } else if (terms[power].Imaginary == 0) {
                    return String.Format("{0}{1}", ParseDouble(terms[power].Real), exponent);
                } else {
                    return String.Format("({0} + {1}i){2}", ParseDouble(terms[power].Real), ParseDouble(terms[power].Imaginary), exponent);
                }
            }));
        }

        private String ParseDouble(Double x)
        {
            if (x != 1) {
                return "(" + x.ToString() + ")";
            } else {
                return "";
            }
        }

        #endregion

        #region Parsing Code

        /// <summary>
        /// Parses an expression into a complex polynomial.
        /// </summary>
        public static Polynomial Parse(String str, IDictionary<String, Double> symbols = null)
        {
            if (symbols == null) {
                symbols = new Dictionary<String, Double>();
            }

            try
            {
                var inputString = str; // for repr field
                str = Regex.Replace(str, @"\s+", "");
                int pos; bool negate = str[0] == '-';
                if (negate) str = str.Substring(1);
                var polynomial = new Polynomial();
                polynomial.repr = inputString;

                do
                {
                    pos = FirstLevelIndexOf(str, new[] { '+', '-' });
                    var exp = pos == -1 ? str : str.Substring(0, pos);
                    var tokens = exp.Split('z'); /* Split and parse */

                    var coeff = ParseComplex(tokens[0], Complex.One, symbols);
                    var power = tokens.Length == 1 ? 0 : (tokens[1] != ""
                              ? UInt32.Parse(tokens[1].Replace("^", ""))
                              : 1);

                    polynomial.AddTerm(power, coeff * (negate ? -1 : 1));

                    if (pos != -1)
                    {
                        negate = str[pos] == '-';
                        str = str.Substring(pos + 1);
                    }
                }
                while (pos != -1);

                return polynomial;
            }
            catch (Exception)
            {
                return null; /* Ugly, but whatever. */
            }
        }

        /// <summary>
        /// Parses an expression into a complex number.
        /// </summary>
        private static Complex ParseComplex(String str, Complex unit, IDictionary<String, Double> symbols)
        {
            if (str == "")
                return unit;
            if (str[0] == '(')
                str = str.Substring(1, str.Length - 2);
            if (symbols.ContainsKey(str))
                return new Complex(symbols[str], 0);
            if (!str.Contains("i")) // real part only
                return new Complex(SafeParse(str, 0, symbols), 0);
            else if (!str.StartsWith("+") && !str.StartsWith("-"))
                str = "+" + str; // add a dummy + to enable split

            if (str.EndsWith("i"))
            {
                var pos = str.LastIndexOfAny(new[] { '+', '-' });
                str = str.Substring(pos) + str.Substring(0, pos);
            }

            return new Complex(SafeParse(str.Split('i')[1], 0, symbols),
                               SafeParse(str.Split('i')[0], 1, symbols));
        }

        private static Double SafeParse(String str, Double unit, IDictionary<String, Double> symbols)
        {
            if (symbols.ContainsKey(str)) {
                return symbols[str];
            }

            switch (str)
            {
                case "":  return unit;
                case "+": return unit;
                case "-": return unit * (-1);
                default:
                    if (str.Contains("/"))
                    {
                        var fraction = str.Split('/');
                        return Double.Parse(fraction[0]) / Double.Parse(fraction[1]);
                    }
                    else
                        return Double.Parse(str);
            }
        }

        /// <summary>
        /// Locates a character in a string not between brackets.
        /// </summary>
        private static int FirstLevelIndexOf(String str, IEnumerable<char> chars)
        {
            int level = 0;

            for (int t = 0; t < str.Length; ++t)
            {
                if (chars.Contains(str[t]) && (level == 0))
                    return t;
                if (str[t] == '(')
                    ++level;
                if (str[t] == ')')
                    --level;
            }

            return -1;
        }

        #endregion
    }
}
