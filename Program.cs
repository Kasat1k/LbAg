using System;
using System.Collections.Generic;

class Program{
    // Функція Ейлера для числа n: повертає кількість чисел, менших за n і взаємно простих з ним
    public static int Phi(int n)
    {
        int result = n;
        for (int p = 2; p * p <= n; p++)
        {
            if (n % p == 0)
            {
                while (n % p == 0)
                    n /= p;
                result -= result / p;
            }
        }
        if (n > 1)
            result -= result / n;
        return result;
    }

    // Функція Мебіуса для числа n: повертає 1, якщо n == 1, 0, якщо n має квадрати, та -1 у всіх інших випадках
    public static int Mu(int n)
    {
        if (n == 1)
            return 1;

        int primeFactors = 0;
        for (int p = 2; p * p <= n; p++)
        {
            if (n % p == 0)
            {
                n /= p;
                primeFactors++;
                if (n % p == 0)
                    return 0;
            }
        }
        if (n > 1)
            primeFactors++;

        return primeFactors % 2 == 1 ? -1 : 1;
    }
    // Обчислення НСК (найменше спільне кратне) для двох чисел a і b
    public static int Lcm(int a, int b)
    {
        return Math.Abs(a * b) / Gcd(a, b);
    }
    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }
    // Обчислення НСК для списку чисел
    public static int LcmList(List<int> numbers)
    {
        int lcmResult = 1;
        foreach (var num in numbers)
        {
            lcmResult = Lcm(lcmResult, num);
        }
        return lcmResult;
    }
    // Обчислення розширеного алгоритму Евкліда для a та b
    public static (int, int, int) ExtendedGcd(int a, int b)
    {
        if (b == 0)
            return (a, 1, 0);

        var (d, x, y) = ExtendedGcd(b, a % b);
        return (d, y, x - (a / b) * y);
    }
    // Функція китайської теореми про залишки для відновлення x за множини залишків {n} та {a}
    public static int ChineseRemainderTheorem(int[] n, int[] a)
    {
        int prod = n.Aggregate(1, (i, j) => i * j);
        int sum = 0;

        for (int i = 0; i < n.Length; i++)
        {
            int p = prod / n[i];
            sum += a[i] * ModularMultiplicativeInverse(p, n[i]) * p;
        }

        return sum % prod;
    }
    // Основний метод, що визначає, чи є число a квадратом за модулем p
    private static int ModularMultiplicativeInverse(int a, int mod)
    {
        var (gcd, x, _) = ExtendedGcd(a, mod);
        if (gcd != 1)
            throw new InvalidOperationException("Modular inverse does not exist.");
        return (x % mod + mod) % mod;
    }

    public static int LegendreSymbol(int a, int p)
    {
        if (a % p == 0) return 0;
        if (PowMod(a, (p - 1) / 2, p) == 1) return 1;
        return -1;
    }
    // Функція обчислення модулярного потужністю
    private static int PowMod(int a, int b, int mod)
    {
        int result = 1;
        a = a % mod;

        while (b > 0)
        {
            if ((b & 1) == 1)
                result = (result * a) % mod;
            b = b >> 1;
            a = (a * a) % mod;
        }
        return result;
    }
    // Функція обчислення символу Якобі для числа а в модулі n
    public static int JacobiSymbol(int a, int n)
    {
        if (n % 2 == 0 || n <= 0) throw new ArgumentException("n must be odd and positive.");

        a = a % n;
        if (a == 0) return 0;
        if (a == 1) return 1;
        if (a == 2)
        {
            switch (n % 8)
            {
                case 3:
                case 5:
                    return -1;
                default:
                    return 1;
            }
        }
        if (a == n - 1)
            return (n % 4 == 1) ? 1 : -1;
        if (Gcd(a, n) > 1) return 0;
        if ((a % 4 == 3) && (n % 4 == 3))
            return -JacobiSymbol(n % a, a);
        return JacobiSymbol(n % a, a);
    }
    // Алгоритм Полларда для швидкого знаходження одного з множників числа n
    public static int PollardRho(int n)
    {
        if ((n & 1) == 0) return 2; // n is even

        int x = 2;
        int y = 2;
        int d = 1;
        Func<int, int> f = num => (num * num + 1) % n;

        while (d == 1)
        {
            x = f(x);
            y = f(f(y));
            d = Gcd(Math.Abs(x - y), n);
        }

        return d;
    }

    // Малий крок - гігантський крок для знаходження дискретного логарифму
    public static int BabyStepGiantStep(int g, int h, int p)
    {
        int m = (int)Math.Sqrt(p) + 1;
        var valueTable = new Dictionary<int, int>();

        // Big step
        for (int j = 0; j < m; j++)
        {
            int value = PowMod(g, j, p);
            valueTable[value] = j;
        }

        // Find the multiplicative inverse of g^m
        int invG = PowMod(g, m * (p - 2), p);
        int currentH = h;

        // Small step
        for (int i = 0; i < m; i++)
        {
            if (valueTable.TryGetValue(currentH, out int j))
            {
                return i * m + j;
            }
            currentH = (int)(((long)currentH * invG) % p);
        }

        throw new InvalidOperationException("No solution found.");
    }

    // Функція, що виконує тест Соловея-Штрассена для перевірки числа на простоту
    public static bool SolovayStrassenTest(int n, int k = 5)
    {
        if (n < 2 || n % 2 == 0) return false;
        if (n == 2 || n == 3) return true;

        Random rnd = new Random();
        for (int i = 0; i < k; i++)
        {
            int a = rnd.Next(2, n - 1);
            int x = JacobiSymbol(a, n);
            if (x == 0 || PowMod(a, (n - 1) / 2, n) != (x + n) % n)
                return false;
        }
        return true;
    }
    // Основна функція для знаходження дискретного квадратного кореня за модулем
    public static int TonelliShanks(int a, int p)
    {
        if (LegendreSymbol(a, p) != 1)
            return -1; // Немає розв'язку

        if (a == 0)
            return 0;
        if (p == 2)
            return a;

        int q = p - 1;
        int s = 0;
        while ((q & 1) == 0)
        {
            s += 1;
            q >>= 1;
        }

        if (s == 1)
        {
            int resu = PowMod(a, (p + 1) / 4, p);
            return resu;
        }

        int z = 2;
        while (LegendreSymbol(z, p) != -1)
            z++;

        int c = PowMod(z, q, p);
        int res = PowMod(a, (q + 1) / 2, p);
        int t = PowMod(a, q, p);
        int m = s;

        while (t != 1)
        {
            int i = 0;
            int temp = t;
            while (temp != 1 && i < m)
            {
                temp = PowMod(temp, 2, p);
                i++;
            }

            int b = PowMod(c, 1 << (m - i - 1), p);
            res = (res * b) % p;
            c = PowMod(b, 2, p);
            t = (t * c) % p;
            m = i;
        }

        return res;
    }


    // Генерація ключів RSA

    public static ((int, int), (int, int)) GenerateRsaKeys(int bits = 256)
    {
        int p = RandomPrime(bits / 2);
        int q = RandomPrime(bits / 2);
        int n = p * q;
        int phiN = (p - 1) * (q - 1);

        Random rnd = new Random();
        int e = rnd.Next(2, phiN - 1);
        while (Gcd(e, phiN) != 1)
            e = rnd.Next(2, phiN - 1);

        var (_, d, _) = ExtendedGcd(e, phiN);
        d = (d % phiN + phiN) % phiN;

        return ((e, n), (d, n));
    }
    // Генерація випадкового простого числа заданого біта
    public static int RandomPrime(int bits)
    {
        Random rnd = new Random();
        while (true)
        {
            int number = rnd.Next(1 << (bits - 1), 1 << bits);
            if (SolovayStrassenTest(number))
                return number;
        }
    }
    // Шифрування RSA
    public static int RsaEncrypt(int message, (int e, int n) publicKey)
    {
        return PowMod(message, publicKey.e, publicKey.n);
    }
    // Розшифрування RSA
    public static int RsaDecrypt(int ciphertext, (int d, int n) privateKey)
    {
        return PowMod(ciphertext, privateKey.d, privateKey.n);
    }
    // Генерація ключів Ель-Гамаля на еліптичних кривих
    public static ((int, int, int), int) GenerateElGamalKeys(int bits = 256)
    {
        int p = RandomPrime(bits);
        int g = FindGenerator(p);
        Random rnd = new Random();
        int x = rnd.Next(1, p - 2);
        int h = PowMod(g, x, p);

        return ((p, g, h), x);
    }

    private static int FindGenerator(int p)
    {
        for (int g = 2; g < p; g++)
        {
            if (IsGenerator(g, p))
                return g;
        }
        throw new InvalidOperationException("Generator not found.");
    }

    private static bool IsGenerator(int g, int p)
    {
        HashSet<int> seen = new HashSet<int>();
        int current = 1;
        for (int i = 0; i < p - 1; i++)
        {
            current = (current * g) % p;
            if (seen.Contains(current))
                return false;
            seen.Add(current);
        }
        return true;
    }
    // Шифрування Ель-Гамаля на еліптичних кривих
    public static (int, int) ElGamalEncrypt(int message, (int p, int g, int h) publicKey)
    {
        Random rnd = new Random();
        int y = rnd.Next(1, publicKey.p - 2);
        int c1 = PowMod(publicKey.g, y, publicKey.p);
        int c2 = (message * PowMod(publicKey.h, y, publicKey.p)) % publicKey.p;
        return (c1, c2);
    }
    // Розшифрування Ель-Гамаля на еліптичних кривих
    public static int ElGamalDecrypt((int, int) ciphertext, int privateKey, (int p, int g, int h) publicKey)
    {
        int s = PowMod(ciphertext.Item1, privateKey, publicKey.p);
        int sInv = ModularMultiplicativeInverse(s, publicKey.p);
        int message = (ciphertext.Item2 * sInv) % publicKey.p;
        return message;
    }

    public class EllipticCurve
    {
        public int A { get; }
        public int B { get; }
        public int P { get; }

        public EllipticCurve(int a, int b, int p)
        {
            A = a;
            B = b;
            P = p;
        }

        public bool IsOnCurve(int x, int y)
        {
            var lhs = (y * y) % P;
            var rhs = (x * x * x + A * x + B) % P;
            return lhs == rhs;
        }

        public (int, int)? AddPoints((int, int)? P, (int, int)? Q)
        {
            if (P == null) return Q;
            if (Q == null) return P;

            var (x1, y1) = P.Value;
            var (x2, y2) = Q.Value;

            if (x1 == x2)
            {
                if (y1 != y2) return null;
                int m = (3 * x1 * x1 + A) * ModularMultiplicativeInverse(2 * y1, P.Value.Item2) % P.Value.Item2;

                int x3 = (m * m - x1 - x2) % P.Value.Item1;
                int y3 = (m * (x1 - x3) - y1) % P.Value.Item1;

                return (x3, y3);
            }
            else
            {
                var diffX = x2 - x1;
                var diffY = y2 - y1;
                int m = diffY * ModularMultiplicativeInverse(diffX, P.Value.Item2) % P.Value.Item2;

                int x3 = (m * m - x1 - x2) % P.Value.Item1;
                int y3 = (m * (x1 - x3) - y1) % P.Value.Item1;

                return (x3, y3);
            }
        }


        public (int, int)? MultiplyPoint((int, int)? P, int n)
        {
            (int, int)? R = null;
            var addend = P;

            while (n > 0)
            {
                if ((n & 1) != 0)
                {
                    R = AddPoints(R, addend);
                }

                addend = AddPoints(addend, addend);
                n >>= 1;
            }

            return R;
        }
    }
    public static ((EllipticCurve, (int, int), (int, int), int), int) GenerateElGamalEcKeys(EllipticCurve curve, (int, int) G, int n)
    {
        var rnd = new Random();
        var x = rnd.Next(1, n - 1);
        var H = curve.MultiplyPoint(G, x);
        return ((curve, G, H.Value, n), x);
    }

    public static ((int, int), (int, int)) ElGamalEcEncrypt((int, int) messagePoint, (EllipticCurve, (int, int), (int, int), int) publicKey)
    {
        var (curve, G, H, n) = publicKey;
        var rnd = new Random();
        var y = rnd.Next(1, n - 1);
        var C1 = curve.MultiplyPoint(G, y).Value;
        var S = curve.MultiplyPoint(H, y).Value;
        var C2 = curve.AddPoints(messagePoint, S).Value;
        return (C1, C2);
    }

    public static (int, int) ElGamalEcDecrypt(((int, int), (int, int)) ciphertext, int privateKey, (EllipticCurve, (int, int), (int, int), int) publicKey)
    {
        var (curve, _, _, _) = publicKey;
        var (C1, C2) = ciphertext;
        var S = curve.MultiplyPoint(C1, privateKey).Value;
        var SInv = (S.Item1, curve.P - S.Item2);
        var M = curve.AddPoints(C2, SInv).Value;
        return M;
    }
    static void Main(string[] args)
    {
        // Функція Ейлера і Мебіуса
        int n = 30;
        Console.WriteLine($"Функція Ейлера для {n}: {Phi(n)}");
        Console.WriteLine($"Функція Мебіуса для {n}: {Mu(n)}");

        // Китайська теорема про залишки
        int[] mods = { 3, 5, 7 };
        int[] residues = { 2, 3, 2 };
        Console.WriteLine($"Рішення системи залишків {String.Join(", ", residues)} по модулях {String.Join(", ", mods)}: {ChineseRemainderTheorem(mods, residues)}");

        // Символ Лежандра і Якобі
        int a = 5, p = 11;
        Console.WriteLine($"Символ Лежандра ({a}/{p}) = {LegendreSymbol(a, p)}");

        // Ро-алгоритм Полларда
        n = 8051;
        Console.WriteLine($"Один із дільників {n} (ро-алгоритм Полларда): {PollardRho(n)}");

        // Большой шаг - малый шаг
        int g = 2, h = 22, modulo = 29;
        Console.WriteLine($"Дискретний логарифм {g}^x = {h} (mod {modulo}): {BabyStepGiantStep(g, h, modulo)}");

        // Алгоритм Тонеллі-Шенкса
        a = 10; p = 13;
        Console.WriteLine($"Дискретний квадратний корінь {a} в GF({p}): {TonelliShanks(a, p)}");

        // RSA
        var (publicKey, privateKey) = GenerateRsaKeys();
        int testMessage = 123456789;
        var ciphertext = RsaEncrypt(testMessage, publicKey);
        var decryptedMessage = RsaDecrypt(ciphertext, privateKey);
        Console.WriteLine($"RSA: Відправлене повідомлення: {testMessage}, Розшифроване повідомлення: {decryptedMessage}");

        // Ель-Гамаль на еліптичних кривих
        var curve = new EllipticCurve(2, 2, 17);
        var G = (5, 1);
        var (publicKeyEc, privateKeyEc) = GenerateElGamalEcKeys(curve, G, 19);
        var testMessagePoint = (9, 16);
        var ciphertextEc = ElGamalEcEncrypt(testMessagePoint, publicKeyEc);
        var decryptedMessagePoint = ElGamalEcDecrypt(ciphertextEc, privateKeyEc, publicKeyEc);
        Console.WriteLine($"Ель-Гамаль на еліптичних кривих: Відправлене повідомлення: ({testMessagePoint.Item1}, {testMessagePoint.Item2}), Розшифроване повідомлення: ({decryptedMessagePoint.Item1}, {decryptedMessagePoint.Item2})");
    }
}