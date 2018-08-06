//build and compilation information refers to last line.
//Note: Please compile it under en-US regional setting.
//Changelog: reconstructed program structure using Object-oriented concepts.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;

namespace LockerManagementSystem
{

	//main class
	public class LockerSystem
	{
		const int TOTAL_LOCKER = 20;
		private static Locker[] arrayOfLocker = new Locker[TOTAL_LOCKER];
		private static List<Application> listOfApplicants = new List<Application> ();

		public Locker[] ArrayOfLocker {
			get { 
				return arrayOfLocker;
			}
			set {
				arrayOfLocker = value;
			}
		}

		public List<Application> ListOfApplicants {
			get {
				return listOfApplicants;
			}
			set {
				listOfApplicants = value;
			}
		}


		public static void Main ()
		{
			InitializeLocker ();
			InitializeApplication ();

			List<int> arrayOfIndex = new List<int>();

			//This method Main() displays user menu.
			//program header
			const string programHeader = "xxxxxx's Locker Management System";	

			for(int count = 0; count < (10+ programHeader.Length +10);count++)
				Console.Write("-");
			Console.WriteLine ();
			Console.WriteLine ("{0,43}",programHeader);
			Console.WriteLine("{0,50}","Developed by The Dream Team, for OOAD Assignment");
			for(int count = 0; count < (10+ programHeader.Length +10);count++)
				Console.Write("-");
			Console.WriteLine ("\n");

			//menu items
			ConsoleKeyInfo input;

			do {
				Console.WriteLine();
				Console.WriteLine ("Welcome! What do you want to do?\n\n");
				Console.WriteLine("1. Register New Applicant");
				Console.WriteLine("2. Assigning Lockers");
				Console.WriteLine("3. Revoke Tenant");
				Console.WriteLine("4. List All Lockers");
				Console.WriteLine("5. List All Pending Applications");
				Console.WriteLine("6. Search Application Status");
				Console.WriteLine("7. Modify Monthly Rental\n\n");
				Console.WriteLine("Press X to exit...");

				for(int count = 0; count < (10+ programHeader.Length +10);count++)
					Console.Write("-");
				Console.WriteLine();

				input = Console.ReadKey ();

				if (input.KeyChar == '1')
				{
					//register new applicant
					CreateApplicant();
					SaveApplication ();
				}
				else if (input.KeyChar == '2')
				{
					//assign locker
					bool successfulAssigned = false;
					bool noRecordFound = false;

					noRecordFound = SearchApplicationStatus(arrayOfIndex);

					if(!noRecordFound)
					{
						successfulAssigned = AssignLocker(arrayOfIndex);

						if(successfulAssigned)
						{
							SaveLocker();
							SaveApplication();
						}
					}

					//Clear search result buffer 
					Console.WriteLine("clearing");
					foreach(int index in arrayOfIndex)
						Console.WriteLine(index);
					arrayOfIndex.Clear();
					Console.WriteLine("cleared");
					foreach(int index in arrayOfIndex)
						Console.WriteLine(index);

				}
				else if (input.KeyChar == '3')
				{
					//revoke tenant
					RevokeTenant();
					SaveLocker();
				}
				else if (input.KeyChar == '4')
					//list all lockers
					ListAllLocker();
				else if (input.KeyChar == '5')

					//list all pending applications
					ListAllApplicants();
				else if (input.KeyChar == '6')
				{
					//search application status
					bool noRecordFound = SearchApplicationStatus(arrayOfIndex);
					foreach(int i in arrayOfIndex)
						Console.WriteLine(i);
					arrayOfIndex.Clear();
				}
				else if (input.KeyChar == '7')
				{ 
					//set monthly rental
					SetMonthlyRental();
					SaveLocker();
				}
				else if (input.Key == ConsoleKey.X)
					Console.WriteLine("\nExiting...");
				else 
					Console.WriteLine("\nInput Error!");
			} while(input.Key != ConsoleKey.X);


		}

		public static void ListAllLocker()
		{
			//allow options to list either empty or assigned lockers

			ConsoleKeyInfo input;

			do 
			{

				Console.WriteLine ("\n1.List all empty locker\n2.List all assigned locker\nPress 'X' to exit.");
				input = Console.ReadKey ();

				if (input.KeyChar == '1') {
					//list array of empty locker 
					Console.WriteLine ("\nList of Empty Locker");

					foreach (Locker locker in arrayOfLocker) {
						if (!locker.LockerAssignedStatus) {
							Console.WriteLine (locker.ToString (true));
						}
					}
				} 
				else if (input.KeyChar == '2') 
				{
					//list array of assigned locker
					Console.WriteLine ("\nList of Assigned Locker");

					foreach (Locker locker in arrayOfLocker) {
						if (locker.LockerAssignedStatus) {
							Console.WriteLine (locker.ToString (false));
						}
					}
				} 
				else if (input.KeyChar == 'x' || input.KeyChar == 'X')
				{
					return;
				}
			} 
			while (input.KeyChar != '1' || input.KeyChar != '2'||input.KeyChar != 'x' || input.KeyChar != 'X');
		}

		public static void SaveLocker()
		{
			write:
			if (File.Exists ("file/lockerDetails.txt"))
			{
				try 
				{
					StreamWriter saveLocker;
					saveLocker = new StreamWriter ("file/lockerDetails.txt");

					Console.WriteLine ("Saving data...");
					for (int lockerCount = 0; lockerCount < TOTAL_LOCKER; lockerCount++) 
					{
						saveLocker.WriteLine (arrayOfLocker [lockerCount].LockerNo);
						saveLocker.WriteLine (arrayOfLocker [lockerCount].LockerSize);
						saveLocker.WriteLine (arrayOfLocker [lockerCount].RentalPerMonth);
						saveLocker.WriteLine (arrayOfLocker [lockerCount].LockerAssignedStatus);
						saveLocker.WriteLine (arrayOfLocker [lockerCount].TenantName);
						saveLocker.WriteLine (arrayOfLocker [lockerCount].TenantIC);
						saveLocker.WriteLine (arrayOfLocker [lockerCount].TenantContactNo);
						saveLocker.WriteLine (arrayOfLocker [lockerCount].TenancyDate);
					}
					Console.WriteLine ("File saved!");

					saveLocker.Close ();
				} 
				catch (System.IO.IOException error) 
				{
					Console.WriteLine (error);
					Console.WriteLine ("Save File Error!!!");
				}
			} 
			else 
			{
				Console.WriteLine ("File doesn't exist.\nCreating file...");
				File.Create("file/lockerDetails.txt");
				Console.WriteLine("File created.");
				goto write;
			}
		}

		public static void InitializeLocker()
		{
			var locate = new CultureInfo("en-US");
			write:
			if (File.Exists ("file/lockerDetails.txt")) 
			{
				try 
				{
					StreamReader readLocker;
					readLocker = new StreamReader ("file/lockerDetails.txt");

					Console.WriteLine ("Loading data... locker details...");
					for (int lockerCount = 0; lockerCount < TOTAL_LOCKER; lockerCount++) 
					{
						arrayOfLocker [lockerCount] = new Locker (lockerCount + 1);

						arrayOfLocker [lockerCount].LockerNo = Convert.ToUInt16 (readLocker.ReadLine ());
						arrayOfLocker [lockerCount].LockerSize = Convert.ToChar (readLocker.ReadLine ());
						arrayOfLocker [lockerCount].RentalPerMonth = Convert.ToUInt16 (readLocker.ReadLine ());
						arrayOfLocker [lockerCount].LockerAssignedStatus = Convert.ToBoolean (readLocker.ReadLine ());
						arrayOfLocker [lockerCount].TenantName = readLocker.ReadLine ();
						arrayOfLocker [lockerCount].TenantIC = readLocker.ReadLine ();
						arrayOfLocker [lockerCount].TenantContactNo = readLocker.ReadLine ();
						arrayOfLocker [lockerCount].TenancyDate = DateTime.Parse (readLocker.ReadLine (),locate);
					}
					Console.WriteLine ("Locker details are successfully loaded.");

					readLocker.Close ();
				} 
				catch (System.IO.IOException error) 
				{
					Console.WriteLine (error);
					Console.WriteLine ("Read File Error!!!");
				}
			}
			else 
			{
				Console.WriteLine ("File doesn't exist.\nCreating file...");
				File.Create("file/lockerDetails.txt");
				Console.WriteLine("File created.");
				//write default locker value
				goto write;
			}
		}

		public static void ListAllApplicants()
		{
			//list arrayList of applicants with unassigned status
			foreach (Application applicationNo in listOfApplicants) 
			{
				if (!applicationNo.AssignedStatus)
					Console.WriteLine (applicationNo.ToString ());
			}

		}

		public static void CreateApplicant()
		{
			//create an object of applicant using constructor
			//1. read collection count
			//2. add object
			//3. write file 

			string tempName, tempIC, tempContact;
			char tempSize;

			Console.WriteLine("Register applicant wizard...\nPlease enter the following information.");

			//data validation needed
			Console.WriteLine ("Applicant Name: ");
			tempName = Console.ReadLine ();

			tempName = tempName.ToUpper ();

			bool isIC = true;
			do
			{
				Console.WriteLine ("Applicant IC Number: ");
				tempIC = Console.ReadLine ();

				isIC = DataValidation.isIC(tempIC);

				if(!isIC)
					Console.WriteLine("Invalid NRIC!");
			}while(!isIC);

			bool isContactNo = true;
			do
			{
				Console.WriteLine ("Applicant Contact Number: ");
				tempContact = Console.ReadLine ();

				isContactNo = DataValidation.isContactNo(tempContact);

				if(!isContactNo)
					Console.WriteLine("Invalid Contact No. Use only Malaysia number!");
			}while(!isContactNo);

			Console.WriteLine("Applicant's preferred locker size: (S, M, L) ");
			tempSize = Convert.ToChar( Console.ReadLine());

			Console.WriteLine(tempSize);

			listOfApplicants.Add (new Application(tempName,tempIC,tempContact,DateTime.Now, tempSize));

		}

		public static bool SearchApplicationStatus(List<int> arrayOfIndex)
		{
			//search applicants by using name, ic or contactNo.
			string searchKeyword;

			Console.WriteLine ("\nSearch Application Status Wizard\nPlease enter your search keyword(by name, contact number or NRIC):");
			searchKeyword = Console.ReadLine ();
			bool found = false; 
			int index = 1;

			searchKeyword = searchKeyword.ToUpper ();

			//search by name
			foreach (Application objCount in listOfApplicants)
			{
				objCount.ApplicantName = objCount.ApplicantName.ToUpper();
				if (objCount.ApplicantName.Contains (searchKeyword)) 
				{
					Console.WriteLine("listOfApplicant : {0}",listOfApplicants.IndexOf(objCount));
					arrayOfIndex.Add (listOfApplicants.IndexOf (objCount));
					//print all results 
					for (int i = 0; i < arrayOfIndex.Count; i++) 
					{
						Console.WriteLine ("{0}" + listOfApplicants[(int)arrayOfIndex[i]].ToString(), index);
						index++;
					}
					found = true;
				} 
			}

			//search by contact number
			foreach (Application objCount in listOfApplicants)
			{
				if (objCount.ApplicantContactNo.Contains (searchKeyword)) 
				{
					arrayOfIndex.Add (listOfApplicants.IndexOf (objCount));
					//print all results 
					for (int i = 0; i < arrayOfIndex.Count; i++) 
					{
						Console.WriteLine ("{0}" + listOfApplicants[(int)arrayOfIndex[i]].ToString(), index);
						index++;
					}
					found = true;
				} 
			}

			//search by NRIC
			foreach (Application objCount in listOfApplicants)
			{
				if (objCount.ApplicantIC.Contains (searchKeyword)) 
				{
					arrayOfIndex.Add (listOfApplicants.IndexOf (objCount));
					//print all results 
					for (int i = 0; i < arrayOfIndex.Count; i++) 
					{
						Console.WriteLine ("{0}" + listOfApplicants[(int)arrayOfIndex[i]].ToString(), index);
						index++;
					}
					found = true;
				} 
			}

			if(!found)
			{
				Console.WriteLine ("Result not found!");
				return true;
			}

			return false;
		}

		public static void InitializeApplication()
		{
			var locate = new CultureInfo("en-US");
			//int applicationCounter = 0;

			Console.WriteLine ("Initialization starting...");

			write:
			if (File.Exists ("file/applicationDetails.txt")) 
			{
				StreamReader readApplication;
				readApplication = new StreamReader ("file/applicationDetails.txt");
				Console.WriteLine ("File exists...");
				try
				{
					if (!readApplication.EndOfStream) 
					{
						Console.WriteLine ("Data initializing...");

						//foreach (Application index in listOfApplication) 
						int objCount = 0;
						do { 
							//Console.WriteLine ("Data reading...");

							listOfApplicants.Add(new Application());

							//Console.WriteLine("An object created..., size is {0}", listOfApplication.Count);

							listOfApplicants[objCount].ApplicantName = readApplication.ReadLine();
							listOfApplicants[objCount].ApplicantIC = readApplication.ReadLine();
							listOfApplicants[objCount].ApplicantContactNo = readApplication.ReadLine();
							listOfApplicants[objCount].ApplicationDate = DateTime.Parse (readApplication.ReadLine(),locate);
							listOfApplicants[objCount].PreferredSize = Convert.ToChar (readApplication.ReadLine());
							listOfApplicants[objCount].AssignedStatus = Convert.ToBoolean(readApplication.ReadLine());

							//Console.WriteLine(_listOfApplication[objCount].ToString());

							objCount++;
						} while (!readApplication.EndOfStream);
					}
					readApplication.Close ();
				}
				catch (System.IO.IOException error)
				{
					Console.WriteLine (error);
					Console.WriteLine ("Read File Error!!!");
				}
			}else
			{
				Console.WriteLine ("File doesn't exist.\nCreating file...");
				File.Create("file/applicationDetails.txt");
				Console.WriteLine("File created.");
				goto write;
			}
		}

		public static void SaveApplication()
		{
			write:
			if (File.Exists ("file/applicationDetails.txt")) 
			{
				StreamWriter	saveApplication;
				saveApplication = new StreamWriter ("file/applicationDetails.txt");
				Console.WriteLine ("Saving...");
				try {
					for (int objCount = 0; objCount < listOfApplicants.Count; objCount++) {
						saveApplication.WriteLine (listOfApplicants [objCount].ApplicantName);
						saveApplication.WriteLine (listOfApplicants [objCount].ApplicantIC);
						saveApplication.WriteLine (listOfApplicants [objCount].ApplicantContactNo);
						saveApplication.WriteLine (listOfApplicants [objCount].ApplicationDate);
						saveApplication.WriteLine (listOfApplicants [objCount].PreferredSize);
						saveApplication.WriteLine (listOfApplicants[objCount].AssignedStatus);
					}

					Console.WriteLine("File Saved!");
					saveApplication.Close ();

				} catch (System.IO.IOException error) {
					Console.WriteLine (error);
					Console.WriteLine ("Write File Error!!!");
				}
			} else 
			{
				Console.WriteLine ("File doesn't exist.\nCreating file...");
				File.Create("file/applicationDetails.txt");
				Console.WriteLine("File created.");
				goto write;
			}
		}
		public static void SetMonthlyRental()
		{
			int lockerNo = 0;
			double rental = 0.0;


			bool isLockerNo = false;
			do
			{
				try
				{
					Console.WriteLine ("Which locker rental do you want to modify? (1-20)");
					lockerNo = Int16.Parse( Console.ReadLine ());

					if(lockerNo >= 0 && lockerNo <= 20 )
						isLockerNo = true;
					else 
						Console.WriteLine("Invalid locker number");
				}
				catch(FormatException invalidInput)
				{
					Console.WriteLine("Invalid locker number");
					Console.WriteLine(invalidInput);
				}

			}while (!isLockerNo);

			bool isMonthlyRental= false;
			do
			{
				try
				{
					Console.WriteLine ("What is the monthly rental for this locker?");
					rental = Double.Parse( Console.ReadLine ());

					if(lockerNo >= 0 )
						isMonthlyRental = true;
					else 
						Console.WriteLine("Invalid figures");
				}
				catch(FormatException invalidInput)
				{
					Console.WriteLine("Invalid figures");
					Console.WriteLine(invalidInput);
				}

			}while (!isMonthlyRental);

			arrayOfLocker [lockerNo-1].RentalPerMonth = rental;	

		}

		public static bool AssignLocker(List<int> arrayOfIndex)
		{
			try {

				Console.WriteLine ("Assign a locker to an applicant");

				int applicantIndex = 1;
				bool isApplicantIndex = false;
				do {
					try {
						Console.WriteLine ("Please select the above applicants by its respective index number: ");
						applicantIndex = Convert.ToInt16 (Console.ReadLine ());
					} catch (FormatException invalidInput) {
						Console.WriteLine ("Invalid figures");
						Console.WriteLine (invalidInput);
					}
					isApplicantIndex = true;
				} while (!isApplicantIndex);

				int locker = 0;
				bool isLockerNo = false;
				do {
					try {
						Console.WriteLine ("Which locker do you want to assign to the above applicant? (1-20)");
						locker = Int16.Parse (Console.ReadLine ());

						if (locker >= 0 && locker <= 20)
						{
							isLockerNo = true;

							if(arrayOfLocker[locker].LockerAssignedStatus)
							{
								isLockerNo = false;
								Console.WriteLine("Locker is occupied! Please choose another locker.");
							}
						}						
						else
							Console.WriteLine ("Invalid locker number");
					} catch (FormatException invalidInput) {
						Console.WriteLine ("Invalid locker number");
						Console.WriteLine (invalidInput);
					}

				} while (!isLockerNo);

				arrayOfLocker [locker - 1].TenantName = listOfApplicants [(int)arrayOfIndex [applicantIndex - 1]].ApplicantName;
				arrayOfLocker [locker - 1].TenantIC = listOfApplicants [(int)arrayOfIndex [applicantIndex - 1]].ApplicantIC;
				arrayOfLocker [locker - 1].TenantContactNo = listOfApplicants [(int)arrayOfIndex [applicantIndex - 1]].ApplicantContactNo;
				arrayOfLocker [locker - 1].TenancyDate = DateTime.Now;
				arrayOfLocker [locker - 1].LockerAssignedStatus = true;
				listOfApplicants [(int)arrayOfIndex [applicantIndex - 1]].AssignedStatus = true;

				Console.WriteLine ("Successfully Assigned.");

				return true;
			} 
			catch (ArgumentOutOfRangeException invalidIndex) 
			{
				Console.WriteLine (invalidIndex);
				Console.WriteLine ("Invalid Locker Number or Invalid Applicant Selected!");

				return false;
			}
		}

		public static void RevokeTenant()
		{
			//render tenant data fields to null or change lockerAssignedStatus flag
			foreach (Locker locker in arrayOfLocker) 
			{
				if (locker.LockerAssignedStatus) 
				{
					Console.WriteLine (locker.ToString (false));
				}
			}

			int lockerNo = 0;
			bool isLockerNo = false;
			do
			{
				try
				{
					Console.WriteLine ("Which tenant you want to revoke? \nPlease input the locker number: (1-20)");
					lockerNo = Int16.Parse( Console.ReadLine ());

					if(lockerNo >= 0 && lockerNo <= 20 )
						isLockerNo = true;
					else 
						Console.WriteLine("Invalid locker number");
				}
				catch(FormatException invalidInput)
				{
					Console.WriteLine("Invalid locker number");
					Console.WriteLine(invalidInput);
				}

			}while (!isLockerNo);

			arrayOfLocker[lockerNo-1].LockerAssignedStatus=false;
			Console.WriteLine("Tenant successfully revoked!");
		}
	}

	//locker class
	public class Locker
	{
		//data field
		private int lockerNo = 0 ;
		private char lockerSize = ' ';
		private double rentalPerMonth = 0; //rental RRP, discount allowed but requires different data field to represent
		private bool lockerAssignedStatus = false;
		private DateTime tenancyDate;
		private string tenantName;
		private string tenantIC;
		private string tenantContactNo;

		//properties
		public int LockerNo
		{
			get
			{
				return lockerNo;
			}
			set 
			{ 
				lockerNo = value;
			}
		}

		public char LockerSize
		{
			get 
			{
				return lockerSize;
			}
			set 
			{
				lockerSize = value;
			}
		}

		public double RentalPerMonth
		{
			get
			{
				return rentalPerMonth;
			}
			set 
			{
				rentalPerMonth = value;
			}
		}

		public DateTime TenancyDate 
		{
			get 
			{
				return tenancyDate;
			}
			set 
			{
				tenancyDate = value;
			}
		}

		public string TenantName 
		{
			get 
			{
				return tenantName;
			}
			set 
			{
				tenantName = value;
			}
		}

		public string TenantIC 
		{
			get 
			{ 
				return tenantIC;
			}
			set 
			{
				tenantIC = value;
			}
		}

		public string TenantContactNo 
		{
			get 
			{
				return tenantContactNo;
			}
			set 
			{
				tenantContactNo = value;
			}
		}

		public bool LockerAssignedStatus 
		{
			get 
			{ 
				return lockerAssignedStatus; 
			}
			set 
			{ 
				lockerAssignedStatus = value;
			}
		}


		//constructor
		public Locker(int _lockerNo, char _size, double _rentalPerMonth, bool _assignedStatus)
		{
			lockerNo = _lockerNo;
			lockerSize = _size;
			rentalPerMonth = _rentalPerMonth;
			lockerAssignedStatus = _assignedStatus;
		}


		public Locker(int _lockerNo, char _size)
		{
			lockerNo = _lockerNo;
			lockerSize = _size;
		}


		public Locker(int _lockerNo)
		{
			lockerNo = _lockerNo;
		}

		public Locker(string _tenantName, string _tenantIC, string _tenantContactNo, DateTime _tenancyDate, int _lockerNo, char _size, double _rentalPerMonth, bool _assignedStatus)
		{
			tenantName = _tenantName;
			tenantIC = _tenantIC;
			tenantContactNo = _tenantContactNo;
			//tenancyDate = ;
			lockerNo = _lockerNo;
			lockerSize = _size;
			rentalPerMonth = _rentalPerMonth;
			lockerAssignedStatus = true;
		}

		//tostring method with parameter
		public string ToString(bool _emptyStatus)
		{
			//if locker is empty, show no tenant details
			if (!_emptyStatus) {
				return "\nLocker No: " + lockerNo + "\nTenant Name: " + tenantName + "\nTenant IC: " + tenantIC +
					"\nTenant Contact Number: " + tenantContactNo + "\nTenancy Start Date: " + tenancyDate +
					"\nLocker Size: " + lockerSize + "\nRentalPerMonth: " + rentalPerMonth + "\n";
			} else {
				return "\nLocker No: " + lockerNo +	"\nLocker Size: " + lockerSize + "\nRentalPerMonth: " + rentalPerMonth + "\n";
			}
		}

	}

	//application class
	public class Application 
	{
		//data field 
		private string applicantName;
		private string applicantIC;
		private string applicantContactNo;
		private DateTime applicationDate;
		private char preferredSize; 
		private bool assignedStatus = false;

		//get properties (used by locker.assignLocker())
		public string ApplicantName 
		{
			get 
			{ 
				return applicantName; 
			}
			set 
			{
				applicantName = value;
			}
		}

		public string ApplicantIC 
		{
			get 
			{ 
				return applicantIC; 
			}
			set 
			{
				applicantIC = value;
			}
		}

		public string ApplicantContactNo 
		{
			get 
			{ 
				return applicantContactNo; 
			}
			set 
			{
				applicantContactNo = value;
			}
		}

		public DateTime ApplicationDate
		{
			get 
			{	
				return applicationDate;
			}
			set 
			{
				applicationDate = value;
			}
		}

		public char PreferredSize
		{
			get
			{
				return preferredSize;
			}
			set
			{
				preferredSize = value;
			}
		}

		//set properties (used by locker.assignLocker())
		public bool AssignedStatus 
		{
			set 
			{ 
				assignedStatus = true; 
			}
			get 
			{
				return assignedStatus;
			}
		}


		//empty constructor
		public Application()
		{
			//empty 
		}

		//constructor (with all information entered)
		public Application(string _applicantName, string _applicantIC, string _applicantContactNo, DateTime _applicationDate, char _size)
		{
			applicantName = _applicantName;
			applicantIC = _applicantIC;
			applicantContactNo = _applicantContactNo;
			applicationDate = DateTime.Now;
			preferredSize = _size;
		}

		//constructor (with all information entered, except size)
		public Application(string _applicantName, string _applicantIC, string _applicantContactNo, DateTime _applicationDate)
		{
			applicantName = _applicantName;
			applicantIC = _applicantIC;
			applicantContactNo = _applicantContactNo;
			applicationDate = DateTime.Now;
			preferredSize = 'S';
		}
			
		//done
		//overriden tostring method
		public override string ToString()
		{
			return "\nApplication Profile" + "\nApplicant Name: " + applicantName + "\nApplicant IC: " + applicantIC +
				"\nApplicant Contact Number: " + applicantContactNo + "\nApplication Date: " + applicationDate +
				"\nPreferred Size: " + preferredSize + "\nAssigned Status: " + assignedStatus;
		}
	}
}

public class DataValidation
{
	public static bool isIC(string _ic)
	{
		//check if ic no. has a valid length.
		//for Malaysian ic only.

		Console.WriteLine ("NRIC entered is : {0}", _ic);
		if (_ic.Length == 14) 
		{
			Console.WriteLine ("IC is 14 digit.");
			//remove dashes from ic
			if (_ic[6] == '-' && _ic[9] == '-') 
			{
				_ic = _ic.Remove (6,1); 
				Console.WriteLine (_ic);
				_ic = _ic.Remove (8,1);
				Console.WriteLine (_ic);
				Console.WriteLine("Dashes removed.");
			} else
				return false;
		}

		Console.WriteLine("Checking if IC is 12 digit. {0}", _ic);
		if (_ic.Length != 12)
			return false;
		Console.WriteLine("IC is 12 digit.");

		Console.WriteLine ("Checking if IC is all numeric values.");
		for (int i = 0; i < _ic.Length; i++)
			if (_ic [i] < '0' || _ic [i] > '9') 
			{
				//Console.WriteLine (i);
				return false;
			}
		Console.WriteLine("IC entered is in all numerical.");

		//validate month in ic no (01-12 only).
		Console.WriteLine("Checking if birth month is valid.");
		if(_ic[2] == '0')
		{
			if (_ic [3] < '0' || _ic [3] > '9')
				return false;					
		}

		if(_ic[2] == '1')
		{
			if (_ic [3] < '0' || _ic [3] > '2')
				return false;					
		}
		Console.WriteLine("Birth month is valid.");

		//validate day in ic no.
		Console.WriteLine("Checking if birth day is valid.");
		if(_ic[4] < '0' || _ic[4] > '3')
		{
			return false;
		}
		Console.WriteLine ("Birth day is valid.");

		//check if day in february exceed the range
		Console.WriteLine("Checking if birth day in February is valid.");
		if ((_ic [2] == '0' && _ic [3] == '2') && _ic [4] == '3') 
		{
			return false;
		}
		Console.WriteLine("Birth day in February is valid.");

		return true;
	}

	public static bool isContactNo(string _contactNo)
	{
		//checking if Contact No. entered is valid
		//for Malaysia number only.

		//remove '+' character if any
		if (_contactNo [0] == '+')
			_contactNo = _contactNo.Remove (0, 1);

		//remove any dash or spacing
		for (int i = 0; i < _contactNo.Length; i++)
			if (_contactNo [i] == ' ' || _contactNo [i] == '-') 
			{
				_contactNo = _contactNo.Remove (i, 1);
			}

		//check if all is in numerical value
		for (int i = 0; i < _contactNo.Length; i++)
			if (_contactNo [i] < '0' || _contactNo [i] > '9') 
			{
				//Console.WriteLine (i);
				return false;
			}

		return true;
	}
}

/*
DEVELOPED USING IDE BELOW:
=== MonoDevelop ===

Version 5.10
Runtime:
Mono 4.2.1 (Debian 4.2.1.102+dfsg2-7ubuntu4) (64-bit)
GTK+ 2.24.30 (Ambiant-MATE theme)

=== Operating System ===

Linux
Linux melvin-PC 4.4.0-31-generic #50-Ubuntu SMP Wed Jul 13 00:07:12 UTC 2016 x86_64 x86_64 x86_64 GNU/Linux
*/

/*
TESTED ON THE PLATFORM BELOW:
Microsoft Visual Studio Enterprise 2015
Version 14.0.25123.00 Update 2
Microsoft .NET Framework
Version 4.6.01055
*/