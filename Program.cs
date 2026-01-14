using System;

class BatalhaNaval
{
    // Tabuleiros
    static char[,] tabuleiro = new char[10, 10];
    static int[,] navio = new int[10, 10];  // Agora: 1=Submarino, 2=Rebocador, 3=Encouraçado, 4=Porta-aviões

    // Configuração dos navios
    static readonly string[] nomesNavios = { "Submarino", "Rebocador", "Encouraçado", "Porta-aviões" };
    static readonly int[] tamanhos = { 1, 2, 3, 4 };  // Total: 10 partes
    static int[] partesAtingidas = new int[4];        // Contador de acertos por navio
    static int naviosAfundados = 0;
    static int tirosRestantes = 20;                   // ATENÇÃO: Com 5 tiros é IMPOSSÍVEL vencer (10 partes necessárias).
                                                    // Usei 20 para tornar jogável. Mude para 5 se quiser desafio extremo/Game Over sempre.

    static void Main(string[] args)
    {
        Console.WriteLine("=== BATALHA NAVAL AVANÇADO ===");
        Console.WriteLine("Afundar: Submarino(1), Rebocador(2), Encouraçado(3), Porta-aviões(4)");
        Console.WriteLine("Digite coordenadas: A1 a J10\n");

        while (true)
        {
            ResetJogo();    // Reseta tudo e posiciona navios
            Jogar();        // Roda o jogo

            // Pergunta jogar novamente
            Console.Write("\nJogar novamente? (s/n): ");
            string? resposta = Console.ReadLine()?.Trim().ToLower();
            if (resposta != "s" && resposta != "sim")
            {
                Console.WriteLine("Obrigado por jogar! 👋");
                break;
            }
            Console.WriteLine();
        }
    }

    static void ResetJogo()
    {
        // Reset tabuleiro
        IniciarTabuleiro();

        // Reset navio (todas posições = 0)
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                navio[i, j] = 0;

        // Reset contadores
        Array.Clear(partesAtingidas, 0, 4);
        naviosAfundados = 0;
        tirosRestantes = 20;  // Mude aqui para 5 se quiser o desafio original

        // Posiciona os 4 navios (sem sobreposição)
        PosicionarTodosNavios();
    }

    static void IniciarTabuleiro()
    {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                tabuleiro[i, j] = '~';
    }

    static void PosicionarTodosNavios()
    {
        for (int tipo = 0; tipo < 4; tipo++)
        {
            PosicionarNavioTamanho(tipo + 1, tamanhos[tipo]);
        }
    }

    static void PosicionarNavioTamanho(int tipo, int tamanho)
    {
        Random rand = new Random();
        bool vertical;
        int linIni, colIni;

        // Loop até encontrar posição livre (sem sobreposição)
        do
        {
            vertical = rand.Next(2) == 0;
            if (vertical)
            {
                linIni = rand.Next(0, 11 - tamanho);  // Garante espaço vertical
                colIni = rand.Next(0, 10);
            }
            else
            {
                linIni = rand.Next(0, 10);
                colIni = rand.Next(0, 11 - tamanho);  // Garante espaço horizontal
            }
        } while (!PosicaoLivre(linIni, colIni, vertical, tamanho));

        // Marca as posições com o TIPO do navio
        for (int i = 0; i < tamanho; i++)
        {
            int l = vertical ? linIni + i : linIni;
            int c = vertical ? colIni : colIni + i;
            navio[l, c] = tipo;
        }
    }

    static bool PosicaoLivre(int lin, int col, bool vertical, int tamanho)
    {
        for (int i = 0; i < tamanho; i++)
        {
            int l = vertical ? lin + i : lin;
            int c = vertical ? col : col + i;
            if (navio[l, c] != 0)  // Já ocupado?
                return false;
        }
        return true;
    }

    static void ImprimirTabuleiro()
    {
        Console.WriteLine("  A B C D E F G H I J");
        for (int i = 0; i < 10; i++)
        {
            Console.Write((i + 1).ToString("D2") + " ");
            for (int j = 0; j < 10; j++)
            {
                Console.Write(tabuleiro[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    static void Jogar()
    {
        while (tirosRestantes > 0 && naviosAfundados < 4)
        {
            ImprimirTabuleiro();
            Console.WriteLine($"Tiros restantes: {tirosRestantes} | Navios afundados: {naviosAfundados}/4");

            Console.Write("Seu tiro: ");
            string? input = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrEmpty(input) || input.Length < 2)
            {
                Console.WriteLine("Coordenada inválida! Ex: A5");
                continue;
            }

            char colunaLetra = input[0];
            string linhaStr = input.Substring(1);

            if (colunaLetra < 'A' || colunaLetra > 'J' || !int.TryParse(linhaStr, out int linhaNum))
            {
                Console.WriteLine("Formato: A1 a J10.");
                continue;
            }

            int coluna = colunaLetra - 'A';
            int linha = linhaNum - 1;

            if (linha < 0 || linha > 9)
            {
                Console.WriteLine("Linha: 1 a 10.");
                continue;
            }

            if (tabuleiro[linha, coluna] != '~')
            {
                Console.WriteLine("Já atirou aqui! Tente outro.");
                continue;
            }

            // 🔥 TIRO VÁLIDO: gasta 1 tiro
            tirosRestantes--;

            // Verifica acerto
            int tipoNavio = navio[linha, coluna];
            if (tipoNavio > 0)
            {
                Console.WriteLine("💥 ACERTOU!");
                tabuleiro[linha, coluna] = 'X';
                partesAtingidas[tipoNavio - 1]++;

                // Verifica se afundou este navio
                if (partesAtingidas[tipoNavio - 1] == tamanhos[tipoNavio - 1])
                {
                    Console.WriteLine($"🎉 VOCÊ AFUNDOU UM {nomesNavios[tipoNavio - 1].ToUpper()}!");
                    naviosAfundados++;
                }
            }
            else
            {
                Console.WriteLine("💦 ÁGUA!");
                tabuleiro[linha, coluna] = 'O';
            }

            Console.WriteLine();  // Espaço visual
        }

        // Fim do jogo
        ImprimirTabuleiro();
        if (naviosAfundados == 4)
        {
            Console.WriteLine("🏆 PARABÉNS! Você afundou TODOS os navios!");
        }
        else
        {
            Console.WriteLine("💀 GAME OVER! Acabaram os tiros.");
            Console.WriteLine($"Você afundou {naviosAfundados} de 4 navios.");
        }

        // DEBUG: Mostrar posições reais dos navios (remova se quiser)
        Console.WriteLine("\n--- POSIÇÕES REAIS DOS NAVIOS (DEBUG) ---");
        for (int tipo = 1; tipo <= 4; tipo++)
        {
            Console.Write($"{nomesNavios[tipo - 1]}: ");
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (navio[i, j] == tipo)
                        Console.Write($"{(char)('A' + j)}{i + 1} ");
            Console.WriteLine();
        }
    }
}