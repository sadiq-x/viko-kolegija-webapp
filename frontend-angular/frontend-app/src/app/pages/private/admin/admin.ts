import { CommonModule, DatePipe } from '@angular/common';
import { Component, computed, signal, TrackByFunction } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/users';
import { ModelEntities } from '../../../models/modelEntity';
import { TeacherService } from '../../../services/teacher';
import { RolesTypes } from '../../../data/roles';
import { EventService } from '../../../services/events';

interface AdminUsers {
  Id: number;
  Username: string;
  Name: string;
  Email: string;
  Image?: string | null;
  NumberPhone: string;
  Role: RolesTypes;
}

interface AdminCourse {
  Id: number;
  Name: string;
  Description: string;
  TopicName: string;
  CreateBy: string;
  Status: string;
  DateCreate: string;
  DateClose: string;
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
})
export class Admin {
  user = signal<ModelEntities[]>([]); //Users information

  limitedRoles: RolesTypes[] = [RolesTypes.User, RolesTypes.Unauthorized]; //List with limited roles for Users and Unauthorized's

  loadingCourses = signal<boolean>(false);

  private allUsers = signal<ModelEntities[]>([]); //List of all users
  private allCourses = signal<AdminCourse[]>([]); //List of all courses

  //Consume the signal allUsers, and return all Users and Unauthorized's
  users = computed<AdminUsers[]>(() => {
    const list = this.allUsers() ?? [];
    return list.filter((u) => u.Role === RolesTypes.User || u.Role === RolesTypes.Unauthorized);
  });
  //Consume the signal allUsers, and return all Teachers
  teachers = computed<AdminUsers[]>(() => {
    return this.allUsers().filter((u) => u.Role === RolesTypes.Teacher);
  });

  usersQuery = signal<string>(''); //Search query Name + Username + Role
  //! TODO
  teachersQuery = signal<string>(''); //Search query Name + Username + Status

  courseQuery = signal<string>(''); //Search Name + Description + Status + Date
  courseStatusFilter = signal<string>(''); // Open / Ongoing / Close
  courseDateFrom = signal<string>(''); // yyyy-MM-dd
  courseDateTo = signal<string>(''); // yyyy-MM-dd

  constructor(private userService: UserService, private eventService: EventService) {}

  ngOnInit(): void {
    this.getUsers();
    this.getCourses();
  }

  //Get the all users from backend
  private getUsers() {
    this.userService.getUsers().subscribe({
      next: (res) => {
        if (!res) {
          this.allUsers.set([]);
          return;
        }

        if (Array.isArray(res)) {
          this.allUsers.set(
            res.map((x: any) => ({
              Id: x.Id ?? x.id,
              Username: x.Username ?? x.username,
              Name: x.Name ?? x.name,
              Email: x.Email ?? x.email,
              Image: x.Image ?? x.image,
              NumberPhone: x.NumberPhone ?? x.numberPhone,
              Address: x.Address ?? x.address,
              Birthday: x.Birthday ?? x.birthday,
              Nationality: x.Nationality ?? x.nationality,
              Gender: x.Gender ?? x.gender,
              Role: x.Role ?? (x.role as RolesTypes),
            }))
          );
        }
      },
    });
  }
  //Get the all courses from backend
  private getCourses() {
    this.loadingCourses.set(true);
    this.eventService.getEvents().subscribe({
      next: (res) => {
        this.loadingCourses.set(false);
        if (!res) {
          this.allCourses.set([]);
          return;
        }

        if (Array.isArray(res)) {
          this.allCourses.set(
            res.map((x: any) => ({
              Id: x.Id ?? x.id,
              Name: x.Name ?? x.name,
              Description: x.Description ?? x.description,
              TopicName: x.TopicName ?? x.topicName,
              CreateBy: x.CreateBy ?? x.createBy,
              Status: x.Status ?? x.status,
              DateCreate: x.DateCreate ?? x.dateCreate,
              DateClose: x.DateClose ?? x.dateClose,
            }))
          );
        }
      },
    });
  }
  //Filter to filter all users User/Unauthorized by name + username + role
  filteredUsers = computed<AdminUsers[]>(() => {
    const list = this.users() ?? [];
    const q = this.normalize(this.usersQuery().trim());

    return list.filter((c) => {
      if (q) {
        const name = this.normalize(c.Name ?? '');
        const username = this.normalize(c.Username ?? '');
        const role = this.normalize(c.Role ?? '');
        if (!name.includes(q) && !username.includes(q) && !role.includes(q)) return false;
      }

      return true;
    });
  });
  //Filter to filter all teachers by name + username
  filteredTeachers = computed<AdminUsers[]>(() => {
    const list = this.teachers() ?? [];
    const q = this.normalize(this.teachersQuery().trim());

    return list.filter((c) => {
      if (q) {
        const name = this.normalize(c.Name ?? '');
        const username = this.normalize(c.Username ?? '');
        if (!name.includes(q) && !username.includes(q)) return false;
      }

      return true;
    });
  });
  //List of all status of the list course
  courseStatusOptions = computed<string[]>(() => {
    const list = this.allCourses() ?? [];
    const set = new Set<string>();

    list.forEach((c) => {
      if (c.Status) {
        set.add(c.Status);
      }
    });

    return Array.from(set).sort(); // opcional: ordena alfabeticamente
  });
  trackByUserId: TrackByFunction<AdminUsers> = (_, item) => item.Id;
  trackByCourseId: TrackByFunction<AdminCourse> = (_, item) => item.Id;
  private normalize(str: string): string {
    return str
      .normalize('NFD')
      .replace(/\p{Diacritic}/gu, '')
      .toLowerCase();
  }
  //To check
  //? read everything below

  // ------- Students-Unauthorized / Teachers -------

  // troca de role vinda do <select> na tabela de estudantes
  onStudentRoleChange(user: AdminUsers, newRole: string) {
    console.log(user);
    console.log(newRole);
    const role = newRole as RolesTypes;

    this.allUsers.update((list) =>
      list.map((u) =>
        u.Id === user.Id
          ? {
              ...u,
              Role: role,
            }
          : u
      )
    );

    // aqui, no futuro:
    // this.adminService.updateUserRole(user.Id, role).subscribe(...)
  }

  filteredCourses = computed<AdminCourse[]>(() => {
    const list = this.allCourses() ?? [];
    const q = this.normalize(this.courseQuery().trim());
    const status = this.courseStatusFilter();
    const from = this.courseDateFrom();
    const to = this.courseDateTo();

    return list.filter((c) => {
      // 1) filtro por texto (Name + Description)
      if (q) {
        const name = this.normalize(c.Name ?? '');
        const desc = this.normalize(c.Description ?? '');
        if (!name.includes(q) && !desc.includes(q)) return false;
      }

      // 2) filtro por status
      if (status && c.Status !== status) return false;

      // 3) filtro por datas (string ISO → Date)
      const courseDate = new Date(c.DateCreate);

      if (from) {
        const dFrom = new Date(from);
        if (courseDate < dFrom) return false;
      }

      if (to) {
        const dTo = new Date(to);
        dTo.setHours(23, 59, 59, 999); // incluir o dia final
        if (courseDate > dTo) return false;
      }

      return true;
    });
  });

  

  //Button to reset all filters of courses
  btnResetCourseFilters() {
    this.courseQuery.set('');
    this.courseStatusFilter.set('');
    this.courseDateFrom.set('');
    this.courseDateTo.set('');
  }
}
