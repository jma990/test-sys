namespace Content_Management_System.Data;

public static class PathDirectory
{
    public const string LoginPage = "/Login";
    public const string LoginCSS = "~/css/login.css";
    public const string LogoutPage = "/Logout";
    public const string MandatoryPasswordChangePage = "/MandatoryPasswordChange";
    public const string ErrorPage = "/Error";

    // Announcements Page and its Partial Views
    public const string AnnouncementsPage = "/Announcements";
    public const string AnnouncementsCSS = "~/css/announcements.css";

    // AdminPosts Page and its Partial Views
    public const string AdminPostsPage = "/AdminPosts";
    public const string _AddAnnouncement = "~/Views/AdminPosts/_AddAnnouncement.cshtml";

    // Departments Page and its Partial Views
    public const string DepartmentsPage = "/Departments";
    public const string _AddDepartment = "~/Views/Departments/_AddDepartment.cshtml";
    public const string _ViewUsers = "~/Views/Departments/_ViewUsers.cshtml";

    // AccountCreation Page and its Partial Views
    public const string AccountCreationPage = "/AccountCreation";
    public const string AccountCreationCSS = "~/css/account-creation.css";

    // UserManagement Page and its Partial Views
    public const string UserManagementPage = "/UserManagement";
    public const string _ResetPasswordLogs = "~/Views/UserManagement/_ResetPasswordLogs.cshtml";

    // AccountSettings Page
    public const string AccountSettingsPage = "/AccountSettings";

    // ChangePersonalInfo Page
    public const string ChangePersonalInfoPage = "/ChangePersonalInfo";

    // ChangePassword Page
    public const string ChangePasswordPage = "/ChangePassword";
    
    // Layout
    public const string LayoutCSS = "~/css/layout.css"; // use layout.css instead

    // Navbar
    public const string NavbarCSS = "~/css/navbar.css";

    // Bootstrap
    public const string BootstrapCSS = "~/lib/bootstrap/dist/css/bootstrap.css";
    public const string BootstrapJS = "~/lib/bootstrap/dist/js/bootstrap.bundle.min.js";

    // Javascript
    public const string SiteJS = "~/js/site.js";

    // Assets
    public const string Logo = "~/assets/logo.png";
}