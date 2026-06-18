using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using System;
using System.Threading;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. הגדרת פרטי ההתחברות ל-Kafka Broker
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "lottery-consumers-group", // מזהה קבוצת הצרכנים (יכול להיות כל שם)
                AutoOffsetReset = AutoOffsetReset.Earliest // קריאת הודעות מתחילת התור במידה וזו פעם ראשונה
            };

            // 2. בניית ה-Consumer באמצעות ה-Builder הרשמי
             var consumer = new ConsumerBuilder<string, string>(config).Build();

            // 3. הרשמה (Subscription) ל-Topic שה-Producer שולח אליו
            string topic = "transaction-events"; // ודא שזה אותו שם Topic בדיוק כמו ב-AppSettings של ה-Producer
            consumer.Subscribe(topic);

            Console.WriteLine($"[SERVER START] Consumer is running and listening for events on topic: '{topic}'...");
            Console.WriteLine("Press Ctrl+C to stop the server.");

            // 4. יצירת מנגנון ביטול וסגירה נקייה כאשר לוחצים Ctrl+C
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                // 5. לולאה אינסופית שרצה וממתינה להודעות חדשות מ-Kafka
                while (!cts.IsCancellationRequested)
                {
                    // הפקודה הזו חוסמת (Blocking) וממתינה עד שמגיעה הודעה חדשה
                    var result = consumer.Consume(cts.Token);

                    // 6. הדפסת ההודעה שהתקבלה ישירות ללוג של הקונסולה
                    Console.WriteLine($"\n-----------------------------------------");
                    Console.WriteLine($"[LOG - {DateTime.Now:yyyy-MM-dd HH:mm:ss}] New Message Received!");
                    Console.WriteLine($"Partition: {result.Partition} | Offset: {result.Offset}");
                    Console.WriteLine($"Key (Gift ID): {result.Message.Key}");
                    Console.WriteLine($"Payload (JSON): {result.Message.Value}");
                    Console.WriteLine($"-----------------------------------------");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n[SERVER STOP] Consumer execution was cancelled by user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                // 7. שחרור המשאבים וסגירה מסודרת מול ה-Broker
                consumer.Close();
                Console.WriteLine("[SERVER CLOSED] Consumer connection closed successfully.");
            }
        }
    }
}

