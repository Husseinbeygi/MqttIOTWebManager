namespace DeviceManagment.Domain.Device
{
    public class User
    {


        public User(string username, string password)
        {
            UserName = username;
            Password = password;
        }

        public string UserName { get; private set; }
        public string Password { get; private set; }

    }
}