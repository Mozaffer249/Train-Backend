using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Data.AppMetaData
{
    public static class Router
    {
        public const string SignleRoute = "/{id}";

        public const string root = "Api";
        public const string version = "V1";
        public const string Rule = root + "/" + version + "/";

        #region Authentication
        public const string Authentication = Rule + "Authentication";
        public const string AuthenticationRegister = Authentication + "/Register";
        public const string AuthenticationLogin = Authentication + "/Login";
        public const string AuthenticationLoginWithTwoFactor = Authentication + "/LoginWithTwoFactor";
        public const string AuthenticationLogout = Authentication + "/Logout";
        public const string AuthenticationRefreshToken = Authentication + "/RefreshToken";
        public const string AuthenticationChangePassword = Authentication + "/ChangePassword";
        public const string AuthenticationSendResetPasswordCode = Authentication + "/SendResetPasswordCode";
        public const string AuthenticationResetPassword = Authentication + "/ResetPassword";
        public const string AuthenticationConfirmEmail = Authentication + "/ConfirmEmail";
        public const string AuthenticationValidateToken = Authentication + "/ValidateToken";
        public const string AuthenticationEnableTwoFactor = Authentication + "/EnableTwoFactor";
        public const string AuthenticationVerifyTwoFactor = Authentication + "/VerifyTwoFactor";
        public const string AuthenticationDisableTwoFactor = Authentication + "/DisableTwoFactor";
        public const string AuthenticationGenerateRecoveryCodes = Authentication + "/GenerateRecoveryCodes";
        public const string AuthenticationGetTwoFactorStatus = Authentication + "/GetTwoFactorStatus";

        // Account Management
        public const string AccountGetProfile = Authentication + "/Profile";
        public const string AccountUpdateProfile = Authentication + "/Profile/Update";
        public const string AccountChangeEmail = Authentication + "/ChangeEmail";
        public const string AccountConfirmEmailChange = Authentication + "/ConfirmEmailChange";
        public const string AccountGetSessions = Authentication + "/Sessions";
        public const string AccountTerminateSession = Authentication + "/Sessions/Terminate";
        public const string AccountTerminateAllSessions = Authentication + "/Sessions/TerminateAll";
        public const string AccountGetTrustedDevices = Authentication + "/TrustedDevices";
        public const string AccountTrustDevice = Authentication + "/TrustDevice";
        public const string AccountRemoveTrustedDevice = Authentication + "/TrustedDevices/Remove";
        public const string AccountGetSecurityEvents = Authentication + "/SecurityEvents";
        public const string AccountExportData = Authentication + "/ExportData";
        public const string AccountDelete = Authentication + "/Delete";
        #endregion

        #region Admin
        public const string Admin = Rule + "Admin";

        // State Management
        public const string AdminStates = Admin + "/States";
        public const string AdminStateById = AdminStates + SignleRoute;

        // City Management
        public const string AdminCities = Admin + "/Cities";
        public const string AdminCityById = AdminCities + SignleRoute;
        public const string AdminCitiesByState = AdminCities + "/ByState/{stateId}";

        // Station Management
        public const string AdminStations = Admin + "/Stations";
        public const string AdminStationById = AdminStations + SignleRoute;
        public const string AdminStationsSearch = AdminStations + "/Search";

        // Train Management
        public const string AdminTrains = Admin + "/Trains";
        public const string AdminTrainById = AdminTrains + SignleRoute;

        // Coach Management
        public const string AdminCoaches = Admin + "/Coaches";
        public const string AdminCoachById = AdminCoaches + SignleRoute;
        public const string AdminCoachesByTrain = AdminCoaches + "/ByTrain/{trainId}";
        public const string AdminCoachesBulkCreate = AdminCoaches + "/Bulk";

        // Seat Management
        public const string AdminSeats = Admin + "/Seats";
        public const string AdminSeatById = AdminSeats + SignleRoute;
        public const string AdminSeatsByCoach = AdminSeats + "/ByCoach/{coachId}";
        public const string AdminSeatsGenerate = AdminSeats + "/Generate/{coachId}";

        // Route Management
        public const string AdminRoutes = Admin + "/Routes";
        public const string AdminRouteById = AdminRoutes + SignleRoute;
        public const string AdminRoutesSearch = AdminRoutes + "/Search";
        public const string AdminRouteStations = AdminRoutes + "/{routeId}/Stations";
        public const string AdminRouteStationById = AdminRouteStations + "/{stationId}";

        // Trip Management
        public const string AdminTrips = Admin + "/Trips";
        public const string AdminTripById = AdminTrips + SignleRoute;
        public const string AdminTripCancel = AdminTrips + "/{id}/Cancel";
        public const string AdminTripInitializeSeats = AdminTrips + "/{id}/InitializeSeats";

        // Fare Management
        public const string AdminFares = Admin + "/Fares";
        public const string AdminFareById = AdminFares + SignleRoute;
        public const string AdminFaresByTrip = AdminFares + "/ByTrip/{tripId}";
        public const string AdminFaresActive = AdminFares + "/Active";

        // User Management (SuperAdmin only)
        public const string AdminUsers = Admin + "/Users";
        public const string AdminUserById = AdminUsers + SignleRoute;
        public const string AdminUserRoles = AdminUsers + "/{userId}/Roles";
        #endregion

        #region Infrastructure
        public const string Infrastructure = Rule + "Infrastructure";

        // Region Management
        public const string InfraRegions = Infrastructure + "/Regions";
        public const string InfraRegionById = InfraRegions + SignleRoute;

        // State Management
        public const string InfraStates = Infrastructure + "/States";
        public const string InfraStateById = InfraStates + SignleRoute;

        // City Management
        public const string InfraCities = Infrastructure + "/Cities";
        public const string InfraCityById = InfraCities + SignleRoute;

        // Station Management
        public const string InfraStations = Infrastructure + "/Stations";
        public const string InfraStationById = InfraStations + SignleRoute;

        // Route Management
        public const string InfraRoutes = Infrastructure + "/Routes";
        public const string InfraRouteById = InfraRoutes + SignleRoute;

        // Train Management
        public const string InfraTrains = Infrastructure + "/Trains";
        public const string InfraTrainById = InfraTrains + SignleRoute;

        // Coach Management
        public const string InfraCoaches = Infrastructure + "/Coaches";
        public const string InfraCoachById = InfraCoaches + SignleRoute;

        // Seat Management
        public const string InfraSeats = Infrastructure + "/Seats";
        public const string InfraSeatById = InfraSeats + SignleRoute;

        // Trip Management
        public const string InfraTrips = Infrastructure + "/Trips";
        public const string InfraTripById = InfraTrips + SignleRoute;
        #endregion

    }
}