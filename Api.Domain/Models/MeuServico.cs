using System;
using Api_Macoratti.Services;

namespace Api_Macoratti.Models
{
    public class MeuServico : IMeuServico
    {
        public string Saudacao(string nome)
        {
            return $"Bem-vindx, {nome} \n\n { DateTime.Now } ";
        }
    }
}