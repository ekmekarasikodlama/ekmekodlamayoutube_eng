start:
            string data = Console.ReadLine();
            if(string.IsNullOrEmpty(data))
            {
                goto start;
            }
            char[] normal = data.ToArray();
            char[] reversed = data.ToArray();
            Array.Reverse(reversed);

            bool palindrome = true;
            for(int i = 0; i < normal.Count(); i++)
            {
                if(normal[i] != reversed[i])
                {
                    palindrome = false;
                }
            }
            Console.WriteLine("Is it a palindrome? : " + palindrome);
            goto start;
