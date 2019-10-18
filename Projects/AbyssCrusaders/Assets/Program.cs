#pragma warning disable 219
namespace AbyssCrusaders
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			using(var main = new Main()) {
				main.Run(args);
			}
		}
	}
}