using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    //db.SubmitChanges();
                    break;
                case "read":
                    Console.WriteLine(employee.FirstName,employee.LastName,employee.EmployeeId);           
                    break;
                case "update":
                    var thing = db.Employees.Where(s => s.EmployeeId == employee.EmployeeId);
                    thing = employee.;//Finish on Wednesday
                    //db.SubmitChanges();
                    break;
                    //Doule check in morning to ensure old value is removed
                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    break;
                 
            }
            db.SubmitChanges();

        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static Animal GetAnimalByID(int id)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {            
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> thing = null;
            foreach(KeyValuePair<int,string> update in updates)
            {
                switch (update.Key)
                { case 1:
                        thing = db.Animals.Where(s => s.Category.Name == update.Value);
                        return thing;
                  case 2:
                        thing = db.Animals.Where(s => s.Name == update.Value);
                        return thing;
                    case 3:
                        thing = db.Animals.Where(s => s.Age == int.Parse(update.Value));
                        return thing;
                    case 4:
                        thing = db.Animals.Where(s => s.Demeanor == update.Value);
                        return thing;
                    case 5:
                        thing = db.Animals.Where(s => s.KidFriendly == bool.Parse(update.Value));
                        return thing;
                    case 6:
                        thing = db.Animals.Where(s => s.PetFriendly == bool.Parse(update.Value));
                        return thing;
                    case 7:
                        thing = db.Animals.Where(s => s.Weight == int.Parse(update.Value));
                        return thing;
                    case 8:
                        thing = db.Animals.Where(s => s.AnimalId == int.Parse(update.Value));
                        return thing;
                    case 9:
                        thing = db.Animals.Where(s => s.Gender == update.Value);
                        break;
                    case 10:
                        thing = db.Animals.Where(s => s.AdoptionStatus == update.Value);
                        return thing;
                    case 11:
                        thing = db.Animals.Where(s => s.CategoryId == int.Parse(update.Value));
                        return thing;
                    case 12:
                        thing = db.Animals.Where(s => s.DietPlanId == int.Parse(update.Value));
                        return thing;
                    case 13:
                        thing = db.Animals.Where(s => s.EmployeeId == int.Parse(update.Value));
                        return thing;
                    //default:
                    //    Console.WriteLine("No match exists");
                    //    return thing;

                }
                return null;
                
            }
            return null;

        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            switch (categoryName)
            {
                case "Cats":
                    return 1;
                case "Dogs":
                    return 2;
                case "Birds":
                    return 3;
                case "Fish":
                    return 4;
                case "Reptiles":
                    return 5;
                default:
                    Console.WriteLine("Input does not correspond to a Known category.");
                    return 0;
            }
            
        }
      
        internal static Room GetRoom(int animalId)
        {
            Room getRoom = db.Rooms.Where(s => s.AnimalId == animalId).SingleOrDefault();
            return getRoom;
            
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            switch (dietPlanName)
            {
                case "KittySlect":
                    return 1;
                case "Wildheart":
                    return 2;
                case "SelectSeed":
                    return 3;
                case "FreshFlakes":
                    return 4;
                case "Live Bait":
                    return 5;
                default:
                    Console.WriteLine("Diet Plan is unavailable");
                    return 0;
            }
            
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}