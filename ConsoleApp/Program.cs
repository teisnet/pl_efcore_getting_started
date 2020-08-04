using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConsoleApp.Tutorial;

namespace ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			// Module2FirstApp.Run();
			// Module5Simple.Run();
			// Module61OneToMany.Run();
			// Module62ManyToMany.Run();
			Module63OneToOne.Run();
		}
	}
}
