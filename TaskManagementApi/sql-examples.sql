USE TaskManagementDb;

SELECT
    t.Id,
    t.Title,
    t.Status,
    t.Priority,
    p.Name as ProjectName,
    u.Name as AssigneeName

FROM Tasks AS t
         INNER JOIN Projects as p ON t.ProjectId = p.Id
         LEFT JOIN Users AS u ON t.AssigneeId = u.Id
ORDER BY t.Id;

-- total count
SELECT COUNT(*) AS TotalCount
FROM Tasks

-- total count with particular status
SELECT COUNT(*) AS TotalCount
FROM Tasks
WHERE Status = 'Pending';


-- Number of tasks per status
SELECT Status, COUNT(*) AS TaskCount
FROM Tasks
GROUP BY Status;


-- should FAIL (project 9999 does not exist)
INSERT INTO Tasks (Title, Description, Status, Priority, DueDate, ProjectId)
VALUES ('Orphan task', '', 'Pending', 'Low', GETUTCDATE(), 9999);


--Cascade: deleting a project deletes its tasks
INSERT INTO Projects (Name, Description, CreatedAt)
VALUES ('Temp project', 'Will be deleted', GETUTCDATE());

DECLARE @ProjectId INT = SCOPE_IDENTITY();

INSERT INTO Tasks (Title, Description, Status, Priority, DueDate, ProjectId)
VALUES ('Temp task', '', 'Pending', 'Low', GETUTCDATE(), @ProjectId);

SELECT * FROM Tasks WHERE ProjectId = @ProjectId;   -- 1 row

DELETE FROM Projects WHERE Id = @ProjectId;

SELECT * FROM Tasks WHERE ProjectId = @ProjectId;