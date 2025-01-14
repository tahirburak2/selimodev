using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Impact_Together.Models;
using System.Data;
using Dapper;

namespace Impact_Together.Controllers;

public class HomeController : Controller
{
private readonly IDbConnection _dbConnection;


    public HomeController(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public IActionResult Index()
    {
        
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        if (ModelState.IsValid)
        {
            var sql = "INSERT INTO Projects (ProjectName, ProjectDescription) VALUES (@ProjectName, @ProjectDescription)";
            await _dbConnection.ExecuteAsync(sql, project);
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }

    public async Task<IActionResult> Edit(int id)
    {
       

        var sql = "SELECT * FROM Projects WHERE Id = @Id";
        var project = await _dbConnection.QueryFirstOrDefaultAsync<Project>(sql, new { Id = id });

        return View(project);
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project)
    {

        if (ModelState.IsValid)
        {
            var sql = "UPDATE Projects SET ProjectName = @ProjectName, ProjectDescription = @ProjectDescription WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, project);
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }

    public async Task<IActionResult> Projects(){
        var sql = "SELECT * FROM Projects"; 
        var projects = await _dbConnection.QueryAsync<Project>(sql); 

        return View(projects);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user)
    {
      
        
            var sql = "INSERT INTO Users (Name, Email, UserType) VALUES (@Name, @Email, @UserType)";
            await _dbConnection.ExecuteAsync(sql, user);
            return RedirectToAction("Index");
        
        
    }

    public IActionResult JoinProject()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> JoinProject(int projectId, string volunteerRole)
    {
        var userId = 1; 

        var sql = "INSERT INTO Volunteers (UserId, ProjectId, VolunteerRole) VALUES (@UserId, @ProjectId, @VolunteerRole)";
        var result = await _dbConnection.ExecuteAsync(sql, new { UserId = userId, ProjectId = projectId, VolunteerRole = volunteerRole });

        if (result > 0)
        {
            return RedirectToAction("Projects", new { id = projectId }); 
        }
        return View("Error"); 
    }
    public async Task<IActionResult> VolunteerDashboard()
    {
        var userId = 1; 
        var sql = "SELECT p.ProjectName AS ProjectName, v.VolunteerRole, v.HoursWorked, v.Contributions FROM Volunteers v " +
                  "JOIN Projects p ON v.ProjectId = p.Id WHERE v.UserId = @UserId";

        var volunteerProjects = await _dbConnection.QueryAsync<VolunteerProject>(sql, new { UserId = userId });
        return View(volunteerProjects);
    }

    public IActionResult UpdateVolunteerProgress()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateVolunteerProgress(int volunteerId, int hoursWorked, string contributions)
    {
        var sql = "UPDATE Volunteers SET HoursWorked = @HoursWorked, Contributions = @Contributions WHERE Id = @VolunteerId";
        var result = await _dbConnection.ExecuteAsync(sql, new { HoursWorked = hoursWorked, Contributions = contributions, VolunteerId = volunteerId });

        if (result > 0)
        {
            return RedirectToAction("VolunteerDashboard");
        }
        return View("Error");

    }
}



