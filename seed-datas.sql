USE schoolmedical_wpf;
GO

-- 1. Xóa bảng phụ thuộc trước
DELETE FROM VaccinationResult;
DELETE FROM HealthCheckResult;
DELETE FROM MedicalEvent;
DELETE FROM HealthProfile;

-- 2. Xóa thông tin kế hoạch
DELETE FROM VaccinationSchedule;
DELETE FROM VaccineDetails;
DELETE FROM HealthCheckSchedule;

-- 3. Xóa học sinh
DELETE MedicalRegistration
DELETE FROM Student;

-- 4. Xóa user đã tạo (chỉ những người tạo ra trong script, ví dụ theo Email)
DELETE FROM [User] WHERE EmailAddress = 'danielleit241@gmail.com';

DECLARE @StaffNurseId UNIQUEIDENTIFIER;

SELECT @StaffNurseId = UserID 
FROM [User] 
WHERE RoleID = 3;

-- 1. Tạo user danielleit241 (parent role)
DECLARE @ParentUserId UNIQUEIDENTIFIER = NEWID()
INSERT INTO [User] (UserId, FullName, PhoneNumber, EmailAddress, PasswordHash, RoleId, Address)
VALUES (
    @ParentUserId,
    N'Daniel Leitão',
    '0987654321',
    'danielleit241@gmail.com',
    'AQAAAAIAAYagAAAAEOITkxUKTMZbjct0+sOzYgq65vYUeNpWhSyoy3s0LHqgU/ihiHnd2fLRpl2k5lckHw==', --parent@123
    4, -- Parent role
    N'123 Trần Hưng Đạo, Quận 1, TP.HCM'
)

-- 2. Tạo students cho parent này
DECLARE @Student1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Student2Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO Student (StudentId, StudentCode, FullName, DayOfBirth, Gender, Grade, Address, ParentPhoneNumber, ParentEmailAddress, UserId)
VALUES 
(
    @Student1Id,
    'ST240001',
    N'Leitão Ana Sofia',
    '2015-03-15',
    N'Nữ',
    '8',
    N'123 Trần Hưng Đạo, Quận 1, TP.HCM',
    '0987654321',
    'danielleit241@gmail.com',
    @ParentUserId
),
(
    @Student2Id,
    'ST240002',
    N'Leitão Miguel João',
    '2017-08-22',
    N'Nam',
    '6',
    N'123 Trần Hưng Đạo, Quận 1, TP.HCM',
    '0987654321',
    'danielleit241@gmail.com',
    @ParentUserId
)

-- 3. Tạo Health Profiles cho students
DECLARE @HealthProfile1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @HealthProfile2Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO HealthProfile (HealthProfileId, StudentId, ChronicDiseases, DrugAllergies, FoodAllergies, Notes, CreatedDate, DeclarationDate)
VALUES 
(
    @HealthProfile1Id,
    @Student1Id,
    N'Không',
    N'Penicillin',
    N'Hải sản',
    N'Cần theo dõi dị ứng thuốc',
    '2024-01-15 08:00:00',
	'2024-01-15'
),
(
    @HealthProfile2Id,
    @Student2Id,
    N'Hen suyễn nhẹ',
    N'Aspirin',
    N'Không',
    N'Cần mang theo thuốc xịt',
    '2024-01-15 08:30:00',
	'2024-01-15'
)

-- 4. Tạo Health Check Schedules
DECLARE @HealthSchedule1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @HealthSchedule2Id UNIQUEIDENTIFIER = NEWID()
DECLARE @HealthSchedule3Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO HealthCheckSchedule (ScheduleId, Title, Description, StartDate, EndDate, HealthCheckType, TargetGrade)
VALUES 
(
    @HealthSchedule1Id,
    N'Khám sức khỏe định kỳ đầu năm học',
    N'Khám tổng quát sức khỏe học sinh',
    '2024-09-15',
	'2024-09-22',
    N'Định kỳ',
    N'Tất cả'
),
(
    @HealthSchedule2Id,
    N'Khám sức khỏe giữa kỳ 1',
    N'Theo dõi sức khỏe học sinh',
    '2024-11-15',
	'2024-11-25',
    N'Định kỳ',
    N'Tất cả'
),
(
    @HealthSchedule3Id,
    N'Khám sức khỏe cuối năm học',
    N'Đánh giá tổng thể sức khỏe',
    '2025-05-15',
	'2024-05-18',
    N'Định kỳ',
    N'Tất cả'
)

-- 5. Tạo Vaccine Details
DECLARE @Vaccine1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Vaccine2Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Vaccine3Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO VaccineDetails (VaccineId, VaccineName, VaccineType, Manufacturer, Description, Disease, AgeRecommendation, BatchNumber, ContraindicationNotes)
VALUES 
(
    @Vaccine1Id,
    N'Vắc xin viêm gan B',
    N'Virus bất hoạt',
    N'Sanofi Pasteur',
    N'Phòng ngừa viêm gan siêu vi B',
    N'Viêm gan B',
    N'Từ sơ sinh',
    N'HBV240315',
    N'Không tiêm cho người dị ứng thành phần'
),
(
    @Vaccine2Id,
    N'Vắc xin DPT',
    N'Kết hợp',
    N'GSK',
    N'Phòng ngừa bạch hầu, ho gà, uốn ván',
    N'Bạch hầu, Ho gà, Uốn ván',
    N'Từ 2 tháng tuổi',
    N'DPT240420',
    N'Không tiêm khi sốt cao'
),
(
    @Vaccine3Id,
    N'Vắc xin cúm mùa',
    N'Virus bất hoạt',
    N'Pfizer',
    N'Phòng ngừa cúm mùa',
    N'Cúm',
    N'Từ 6 tháng tuổi',
    N'FLU240901',
    N'Không tiêm cho người dị ứng trứng gà'
)

-- 6. Tạo Vaccination Schedules
DECLARE @VaccSchedule1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @VaccSchedule2Id UNIQUEIDENTIFIER = NEWID()
DECLARE @VaccSchedule3Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO VaccinationSchedule (ScheduleId, Title, Description, StartDate, EndDate, VaccineId, TargetGrade, Round)
VALUES 
(
    @VaccSchedule1Id,
    N'Tiêm chủng viêm gan B',
    N'Tiêm bổ sung vắc xin viêm gan B',
    '2024-10-01',
	'2024-10-07',
	@Vaccine1Id,
    N'6-8',
    N'Bổ sung'
),
(
    @VaccSchedule2Id,
    N'Tiêm chủng DPT',
    N'Tiêm nhắc lại DPT',
    '2024-12-15',
	'2024-12-22',
    @Vaccine2Id,
    N'6-8',
    N'Nhắc lại'
),
(
    @VaccSchedule3Id,
    N'Tiêm phòng cúm mùa 2024',
    N'Tiêm phòng cúm mùa cho học sinh',
    '2024-12-01',
	'2024-12-07',
    @Vaccine3Id,
    N'Tất cả',
    N'Hàng năm'
)

-- 7. Tạo Health Check Results với kiểu dữ liệu chính xác
INSERT INTO HealthCheckResult (ResultId, HealthProfileId, ScheduleId, DatePerformed, Height, Weight, VisionLeft, VisionRight, Hearing, Nose, BloodPressure, Notes, RecordedID)
VALUES 
-- Ana Sofia - Lần 1 (Sept 2024)
(
    NEWID(),
    @HealthProfile1Id,
    @HealthSchedule1Id,
    '2024-09-15',
    145.5,              -- float
    38.2,               -- float  
    10.0,               -- float (thị lực 10/10)
    10.0,               -- float (thị lực 10/10)
    N'Bình thường',     -- nvarchar
    N'Bình thường',     -- nvarchar
    N'110/70',          -- nvarchar
    N'Sức khỏe tốt, phát triển bình thường theo độ tuổi',
	@StaffNurseId
),
-- Ana Sofia - Lần 2 (Nov 2024)
(
    NEWID(),
    @HealthProfile1Id,
    @HealthSchedule2Id,
    '2024-11-15',
    147.0,              -- float
    39.1,               -- float
    10.0,               -- float
    10.0,               -- float
    N'Bình thường',
    N'Bình thường',
    N'112/72',
    N'Phát triển tốt, chiều cao cân nặng tăng đều',
	@StaffNurseId
),
-- Ana Sofia - Lần 3 (May 2025)
(
    NEWID(),
    @HealthProfile1Id,
    @HealthSchedule3Id,
    '2025-05-15',
    149.2,              -- float
    40.8,               -- float
    10.0,               -- float
    10.0,               -- float
    N'Bình thường',
    N'Bình thường',
    N'115/75',
    N'Sức khỏe tốt, duy trì chế độ tránh dị ứng hải sản',
	@StaffNurseId
),
-- Miguel João - Lần 1 (Sept 2024)
(
    NEWID(),
    @HealthProfile2Id,
    @HealthSchedule1Id,
    '2024-09-15',
    125.8,              -- float
    28.5,               -- float
    10.0,               -- float
    9.0,                -- float (thị lực phải hơi kém)
    N'Bình thường',
    N'Hơi nghẹt',
    N'95/60',
    N'Cần theo dõi hen suyễn, thị lực mắt phải cần kiểm tra thêm',
	@StaffNurseId
),
-- Miguel João - Lần 2 (Nov 2024)
(
    NEWID(),
    @HealthProfile2Id,
    @HealthSchedule2Id,
    '2024-11-15',
    127.2,              -- float
    29.3,               -- float
    10.0,               -- float
    9.5,                -- float (thị lực phải cải thiện)
    N'Bình thường',
    N'Bình thường',
    N'98/62',
    N'Hen suyễn được kiểm soát tốt, thị lực cải thiện',
	@StaffNurseId
),
-- Miguel João - Lần 3 (May 2025)
(
    NEWID(),
    @HealthProfile2Id,
    @HealthSchedule3Id,
    '2025-05-15',
    130.5,              -- float
    31.2,               -- float
    10.0,               -- float
    10.0,               -- float (thị lực đã bình thường)
    N'Bình thường',
    N'Bình thường',
    N'100/65',
    N'Tình trạng hen suyễn ổn định, thị lực đã bình thường',
	@StaffNurseId
)

-- 8. Tạo Vaccination Results
INSERT INTO VaccinationResult (VaccinationResultId, HealthProfileId, ScheduleId, VaccinationDate, DoseNumber, InjectionSite, ImmediateReaction, ReactionType, SeverityLevel, Notes, RecordedID)
VALUES 
-- Ana Sofia - Vaccination Records
(
    NEWID(),
    @HealthProfile1Id,
    @VaccSchedule1Id,
    '2024-10-01',
    1,
    N'Cánh tay trái',
    N'Không',
    N'Không có phản ứng',
    N'Không',
    N'Tiêm thành công, không có phản ứng bất thường',
	@StaffNurseId
),
(
    NEWID(),
    @HealthProfile1Id,
    @VaccSchedule2Id,
    '2024-10-15',
    1,
    N'Cánh tay phải',
    N'Sưng nhẹ',
    N'Phản ứng tại chỗ',
    N'Nhẹ',
    N'Sưng nhẹ tại vị trí tiêm, tự khỏi sau 2 ngày',
	@StaffNurseId
),
(
    NEWID(),
    @HealthProfile1Id,
    @VaccSchedule3Id,
    '2024-12-01',
    1,
    N'Cánh tay trái',
    N'Không',
    N'Không có phản ứng',
    N'Không',
    N'Tiêm cúm thành công, không có phản ứng',
	@StaffNurseId
),
-- Miguel João - Vaccination Records
(
    NEWID(),
    @HealthProfile2Id,
    @VaccSchedule1Id,
    '2024-10-01',
    1,
    N'Cánh tay trái',
    N'Không',
    N'Không có phản ứng',
    N'Không',
    N'Tiêm thành công, theo dõi thêm do hen suyễn',
	@StaffNurseId
),
(
    NEWID(),
    @HealthProfile2Id,
    @VaccSchedule2Id,
    '2024-10-15',
    1,
    N'Cánh tay phải',
    N'Không',
    N'Không có phản ứng',
    N'Không',
    N'Tiêm DPT thành công, không ảnh hưởng đến hen suyễn',
	@StaffNurseId
),
(
    NEWID(),
    @HealthProfile2Id,
    @VaccSchedule3Id,
    '2024-12-01',
    1,
    N'Cánh tay trái',
    N'Không',
    N'Không có phản ứng',
    N'Không',
    N'Tiêm cúm thành công, không ảnh hưởng hen suyễn',
	@StaffNurseId
)

-- 10. TẠO MEDICAL EVENTS CHO 2 HỌC SINH
-- Sử dụng Student IDs đã tạo ở trên thay vì hardcode

-- Medical Events cho Student 1 (Ana Sofia) - @Student1Id
INSERT INTO MedicalEvent (EventId, StudentId, EventType, EventDescription, EventDate, Location, SeverityLevel, StaffNurseId, Notes, ParentNotified)
VALUES 
-- Event 1: Đau đầu nhẹ
(
    NEWID(),
    @Student1Id,
    N'Khám bệnh',
    N'Đau đầu nhẹ, có thể do áp lực học tập',
    '2024-11-25 09:15:00',
    N'Phòng y tế trường',
    N'Nhẹ',
    @StaffNurseId,
    N'Cho nghỉ ngơi 30 phút, uống nước. Triệu chứng giảm sau đó.',
	0
),
-- Event 2: Ngã xe đạp
(
    NEWID(),
    @Student1Id,
    N'Tai nạn',
    N'Ngã xe đạp trong sân trường, trầy xước đầu gối',
    '2024-12-03 14:30:00',
    N'Sân trường',
    N'Trung bình',
    @StaffNurseId,
    N'Rửa vết thương, băng bó. Thông báo phụ huynh theo dõi.',
	1
),
-- Event 3: Dị ứng thức ăn
(
    NEWID(),
    @Student1Id,
    N'Cấp cứu',
    N'Dị ứng hải sản trong bữa trưa, nổi mẩn đỏ',
    '2024-12-10 12:45:00',
    N'Phòng y tế trường',
    N'Trung bình',
    @StaffNurseId,
    N'Sử dụng thuốc kháng histamine, liên lạc ngay với phụ huynh. Đã ổn định.',
	1
),
-- Event 4: Khám định kỳ
(
    NEWID(),
    @Student1Id,
    N'Khám bệnh',
    N'Khám sức khỏe định kỳ, kiểm tra dị ứng',
    '2025-01-15 10:00:00',
    N'Phòng y tế trường',
    N'Nhẹ',
    @StaffNurseId,
    N'Sức khỏe tốt, nhắc nhở tránh hải sản.',
	0
),
-- Event 5: Đau bụng nhẹ
(
    NEWID(),
    @Student1Id,
    N'Khám bệnh',
    N'Đau bụng nhẹ sau bữa trưa',
    '2025-03-20 13:15:00',
    N'Phòng y tế trường',
    N'Nhẹ',
    @StaffNurseId,
    N'Nghỉ ngơi 30 phút, triệu chứng giảm. Có thể do ăn nhanh.',
	0
),
-- Event 6: Gần thời điểm hiện tại
(
    NEWID(),
    @Student1Id,
    N'Khám bệnh',
    N'Đau đầu nhẹ do căng thẳng trước kỳ thi',
    '2025-06-20 14:30:00',
    N'Phòng y tế trường',
    N'Nhẹ',
    @StaffNurseId,
    N'Tư vấn về quản lý căng thẳng, cho nghỉ ngơi. Đã khỏi.',
	0
)

-- Medical Events cho Student 2 (Miguel João) - @Student2Id
INSERT INTO MedicalEvent (EventId, StudentId, EventType, EventDescription, EventDate, Location, SeverityLevel, StaffNurseId, Notes, ParentNotified)
VALUES 
-- Event 1: Cơn hen suyễn nghiêm trọng
(
    NEWID(),
    @Student2Id,
    N'Cấp cứu',
    N'Cơn hen suyễn trong giờ thể dục',
    '2024-11-20 10:30:00',
    N'Sân thể dục',
    N'Nghiêm trọng',
    @StaffNurseId,
    N'Sử dụng thuốc xịt bronchodilator, gọi cấp cứu. Đã ổn định sau 15 phút.',
	1
),
-- Event 2: Đau bụng
(
    NEWID(),
    @Student2Id,
    N'Khám bệnh',
    N'Đau bụng sau bữa sáng',
    '2024-12-05 09:45:00',
    N'Phòng y tế trường',
    N'Nhẹ',
    @StaffNurseId,
    N'Nghỉ ngơi, uống nước ấm. Triệu chứng giảm sau 1 giờ.',
	1
),
-- Event 3: Hen suyễn nhẹ do thời tiết
(
    NEWID(),
    @Student2Id,
    N'Khám bệnh',
    N'Khó thở nhẹ do thời tiết lạnh, không khí khô',
    '2024-12-15 08:20:00',
    N'Phòng học',
    N'Trung bình',
    @StaffNurseId,
    N'Sử dụng thuốc xịt dự phòng, cho về nhà nghỉ nửa ngày.',
	1
),
-- Event 4: Kiểm tra định kỳ hen suyễn
(
    NEWID(),
    @Student2Id,
    N'Khám bệnh',
    N'Kiểm tra tình trạng hen suyễn định kỳ',
    '2025-01-08 11:00:00',
    N'Phòng y tế trường',
    N'Nhẹ',
    @StaffNurseId,
    N'Tình trạng hen suyễn được kiểm soát tốt. Tiếp tục theo dõi.',
	0
),
-- Event 5: Chấn thương nhẹ khi chơi
(
    NEWID(),
    @Student2Id,
    N'Tai nạn',
    N'Bị bạn va chạm khi chơi, sưng nhẹ cổ tay',
    '2025-02-12 15:10:00',
    N'Sân chơi',
    N'Nhẹ',
    @StaffNurseId,
    N'Chườm lạnh, băng đàn hồi. Theo dõi trong 24h.',
	0
),
-- Event 6: Cảm lạnh
(
    NEWID(),
    @Student2Id,
    N'Khám bệnh',
    N'Triệu chứng cảm lạnh: sổ mũi, ho nhẹ',
    '2025-03-05 08:45:00',
    N'Phòng y tế trường',
    N'Nhẹ',
    @StaffNurseId,
    N'Đo nhiệt độ bình thường. Khuyên về nhà nghỉ ngơi, uống thuốc cảm.',
	0
),
-- Event 7: Hen suyễn do gió bụi
(
    NEWID(),
    @Student2Id,
    N'Khám bệnh',
    N'Khó thở nhẹ do tiếp xúc với bụi trong lúc dọn dẹp lớp',
    '2025-04-18 16:20:00',
    N'Phòng học',
    N'Trung bình',
    @StaffNurseId,
    N'Sử dụng thuốc xịt, tránh tiếp xúc với allergen. Về nhà sớm.',
	1
),
-- Event 8: Chấn thương nhẹ trong thể dục
(
    NEWID(),
    @Student2Id,
    N'Tai nạn',
    N'Bị trượt ngã khi chạy, trầy xước đầu gối',
    '2025-05-10 10:45:00',
    N'Sân thể dục',
    N'Nhẹ',
    @StaffNurseId,
    N'Rửa vết thương, băng bó. Theo dõi để tránh nhiễm trùng.',
	1
),
-- Event 9: Gần thời điểm hiện tại
(
    NEWID(),
    @Student2Id,
    N'Khám bệnh',
    N'Kiểm tra hen suyễn do thời tiết nóng',
    '2025-06-22 10:15:00',
    N'Phòng y tế trường',
    N'Trung bình',
    @StaffNurseId,
    N'Khuyến cáo tránh vận động mạnh trong thời tiết nóng. Mang theo thuốc xịt.',
	1
)

-- =====================================================
-- PHẦN KIỂM TRA KẾT QUẢ VÀ THỐNG KÊ
-- =====================================================

PRINT '✅ ✅ ✅ TẠO DỮ LIỆU HOÀN CHỈNH THÀNH CÔNG! ✅ ✅ ✅'
PRINT ''
PRINT '=================================================='
PRINT 'THÔNG TIN TÀI KHOẢN VÀ DỮ LIỆU ĐÃ TẠO'
PRINT '=================================================='

-- Thông tin tài khoản
SELECT '🔐 THÔNG TIN TÀI KHOẢN' as [Section]
SELECT 
    u.FullName as [Họ tên],
    u.EmailAddress as [Email],
    'parent@123' as [Mật khẩu],
    r.RoleName as [Vai trò],
    u.PhoneNumber as [Số điện thoại],
    u.Address as [Địa chỉ]
FROM [User] u
JOIN Role r ON u.RoleId = r.RoleId
WHERE u.EmailAddress = 'danielleit241@gmail.com'

-- Danh sách học sinh và ID
SELECT '👨‍🎓 DANH SÁCH HỌC SINH VÀ ID' as [Section]
SELECT 
    s.StudentId as [Student ID],
    s.StudentCode as [Mã số],
    s.FullName as [Họ tên],
    s.Grade as [Lớp],
    s.Gender as [Giới tính],
    s.DayOfBirth as [Ngày sinh]
FROM Student s
JOIN [User] u ON s.UserId = u.UserId
WHERE u.EmailAddress = 'danielleit241@gmail.com'
ORDER BY s.FullName

-- Thống kê tổng quan
SELECT '📊 THỐNG KÊ TỔNG QUAN' as [Section]
SELECT 
    (SELECT COUNT(*) FROM HealthCheckResult hr 
     JOIN HealthProfile hp ON hr.HealthProfileId = hp.HealthProfileId
     JOIN Student s ON hp.StudentId = s.StudentId
     JOIN [User] u ON s.UserId = u.UserId
     WHERE u.EmailAddress = 'danielleit241@gmail.com') as [Kết quả khám sức khỏe],
    
    (SELECT COUNT(*) FROM VaccinationResult vr 
     JOIN HealthProfile hp ON vr.HealthProfileId = hp.HealthProfileId
     JOIN Student s ON hp.StudentId = s.StudentId
     JOIN [User] u ON s.UserId = u.UserId
     WHERE u.EmailAddress = 'danielleit241@gmail.com') as [Kết quả tiêm chủng],
     
    (SELECT COUNT(*) FROM MedicalEvent me 
     JOIN Student s ON me.StudentId = s.StudentId
     JOIN [User] u ON s.UserId = u.UserId
     WHERE u.EmailAddress = 'danielleit241@gmail.com') as [Sự kiện y tế]

-- Chi tiết Medical Events theo học sinh
SELECT '🚨 MEDICAL EVENTS - ANA SOFIA' as [Section]
SELECT 
    s.FullName as [Học sinh],
    s.Grade as [Lớp],
    FORMAT(me.EventDate, 'dd/MM/yyyy HH:mm') as [Ngày giờ],
    me.EventType as [Loại sự kiện],
    me.EventDescription as [Mô tả],
    me.SeverityLevel as [Mức độ],
    me.Location as [Địa điểm],
    me.Notes as [Ghi chú]
FROM MedicalEvent me
JOIN Student s ON me.StudentId = s.StudentId
JOIN [User] u ON s.UserId = u.UserId
WHERE u.EmailAddress = 'danielleit241@gmail.com' 
  AND s.FullName LIKE N'%Ana Sofia%'
ORDER BY me.EventDate DESC

SELECT '🚨 MEDICAL EVENTS - MIGUEL JOÃO' as [Section]
SELECT 
    s.FullName as [Học sinh],
    s.Grade as [Lớp],
    FORMAT(me.EventDate, 'dd/MM/yyyy HH:mm') as [Ngày giờ],
    me.EventType as [Loại sự kiện],
    me.EventDescription as [Mô tả],
    me.SeverityLevel as [Mức độ],
    me.Location as [Địa điểm],
    me.Notes as [Ghi chú]
FROM MedicalEvent me
JOIN Student s ON me.StudentId = s.StudentId
JOIN [User] u ON s.UserId = u.UserId
WHERE u.EmailAddress = 'danielleit241@gmail.com' 
  AND s.FullName LIKE N'%Miguel%'
ORDER BY me.EventDate DESC

-- Thống kê Medical Events
SELECT '📈 THỐNG KÊ MEDICAL EVENTS' as [Section]
SELECT 
    COUNT(*) as [Tổng số sự kiện],
    COUNT(DISTINCT me.StudentId) as [Số học sinh có sự kiện],
    SUM(CASE WHEN me.SeverityLevel = N'Nghiêm trọng' THEN 1 ELSE 0 END) as [Sự kiện nghiêm trọng],
    SUM(CASE WHEN me.SeverityLevel = N'Trung bình' THEN 1 ELSE 0 END) as [Sự kiện trung bình],
    SUM(CASE WHEN me.SeverityLevel = N'Nhẹ' THEN 1 ELSE 0 END) as [Sự kiện nhẹ]
FROM MedicalEvent me
JOIN Student s ON me.StudentId = s.StudentId
JOIN [User] u ON s.UserId = u.UserId
WHERE u.EmailAddress = 'danielleit241@gmail.com'

-- Thống kê theo loại sự kiện
SELECT '📋 THỐNG KÊ THEO LOẠI SỰ KIỆN' as [Section]
SELECT 
    me.EventType as [Loại sự kiện],
    COUNT(*) as [Số lượng],
    COUNT(DISTINCT me.StudentId) as [Số học sinh]
FROM MedicalEvent me
JOIN Student s ON me.StudentId = s.StudentId  
JOIN [User] u ON s.UserId = u.UserId
WHERE u.EmailAddress = 'danielleit241@gmail.com'
GROUP BY me.EventType
ORDER BY COUNT(*) DESC

-- Lịch sử khám sức khỏe gần đây
SELECT '🏥 LỊCH SỬ KHÁM SỨC KHỎE GỦI ĐÂY' as [Section]
SELECT TOP 5
    s.FullName as [Học sinh],
    s.Grade as [Lớp],
    FORMAT(hr.DatePerformed, 'dd/MM/yyyy') as [Ngày khám],
    hr.Height as [Chiều cao (cm)],
    hr.Weight as [Cân nặng (kg)],
    hr.Notes as [Ghi chú]
FROM HealthCheckResult hr
JOIN HealthProfile hp ON hr.HealthProfileId = hp.HealthProfileId
JOIN Student s ON hp.StudentId = s.StudentId
JOIN [User] u ON s.UserId = u.UserId
WHERE u.EmailAddress = 'danielleit241@gmail.com'
ORDER BY hr.DatePerformed DESC

-- Lịch sử tiêm chủng gần đây
SELECT '💉 LỊCH SỬ TIÊM CHỦNG GỬI ĐÂY' as [Section]
SELECT TOP 5
    s.FullName as [Học sinh],
    s.Grade as [Lớp],
    FORMAT(vr.VaccinationDate, 'dd/MM/yyyy') as [Ngày tiêm],
    vd.VaccineName as [Tên vaccine],
    vr.DoseNumber as [Liều],
    vr.Notes as [Ghi chú]
FROM VaccinationResult vr
JOIN HealthProfile hp ON vr.HealthProfileId = hp.HealthProfileId
JOIN Student s ON hp.StudentId = s.StudentId
JOIN [User] u ON s.UserId = u.UserId
JOIN VaccinationSchedule vs ON vr.ScheduleId = vs.ScheduleId
JOIN VaccineDetails vd ON vs.VaccineId = vd.VaccineId
WHERE u.EmailAddress = 'danielleit241@gmail.com'
ORDER BY vr.VaccinationDate DESC

PRINT ''
PRINT '=================================================='
PRINT '✅ HOÀN TẤT TẤT CẢ! SẴN SÀNG TEST HEALTH HISTORY PAGE'
PRINT 'Login: danielleit241@gmail.com'
PRINT 'Password: parent@123'
PRINT '=================================================='