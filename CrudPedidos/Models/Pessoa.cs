using System;

namespace CrudPedidos.Models
{
    public class Pessoa
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string Status { get; set; } = "A";

        public Pessoa() { }

        public Pessoa(int id, string nome, string cpf, string status = "A")
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Status = status;
            Validar();
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new ArgumentException("Nome é obrigatório.");

            if (!ValidarCpf(Cpf))
                throw new ArgumentException("CPF inválido.");
        }

        public static bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;
            var digits = SomenteNumeros(cpf);
            if (digits.Length != 11) return false;
            if (new string(digits[0], 11) == digits) return false;

            int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string temp = digits.Substring(0, 9);
            int soma = 0;
            for (int i = 0; i < 9; i++) soma += (temp[i] - '0') * mult1[i];
            int resto = soma % 11;
            int dig1 = resto < 2 ? 0 : 11 - resto;

            temp += dig1.ToString();
            soma = 0;
            for (int i = 0; i < 10; i++) soma += (temp[i] - '0') * mult2[i];
            resto = soma % 11;
            int dig2 = resto < 2 ? 0 : 11 - resto;

            return digits[9] - '0' == dig1 && digits[10] - '0' == dig2;
        }

        public static string SomenteNumeros(string valor)
        {
            char[] arr = new char[valor.Length];
            int idx = 0;
            for (int i = 0; i < valor.Length; i++)
            {
                char c = valor[i];
                if (c >= '0' && c <= '9')
                {
                    arr[idx++] = c;
                }
            }
            return new string(arr, 0, idx);
        }

        public static string FormatCpf(string cpf)
        {
            string d = SomenteNumeros(cpf ?? string.Empty);
            if (d.Length != 11) return cpf ?? string.Empty;
            return string.Format("{0}.{1}.{2}-{3}", d.Substring(0, 3), d.Substring(3, 3), d.Substring(6, 3), d.Substring(9, 2));
        }

        public string StatusDescricao => Status == "A" ? "Ativo" : "Inativo";
    }
}
