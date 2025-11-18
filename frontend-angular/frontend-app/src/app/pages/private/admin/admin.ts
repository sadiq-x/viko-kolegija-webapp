import { CommonModule, DatePipe } from '@angular/common';
import { Component, computed, signal, TrackByFunction } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../../services/users';
import { ModelEntities } from '../../../models/modelEntity';
import { RolesTypes } from '../../../data/roles';
import { EventService } from '../../../services/events';
import { RouterLink } from '@angular/router';
import { TopicsService } from '../../../services/topics';
import { ModelTopicsResponse } from '../../../models/modelTopics';
import { RolesService } from '../../../services/roles';

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

interface AdminTopics {
  Id: number;
  Type: string;
  Description: string;
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, RouterLink, ReactiveFormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
})
export class Admin {
  user = signal<ModelEntities[]>([]); //Users information

  typesRoles: RolesTypes[] = [
    RolesTypes.User,
    RolesTypes.Unauthorized,
    RolesTypes.Teacher,
    RolesTypes.Admin,
  ]; //List with limited roles for Users and Unauthorized's

  loadingCourses = signal<boolean>(false); //Boolean variable to load view "loading"

  private allUsers = signal<ModelEntities[]>([]); //List of all users
  private allCourses = signal<AdminCourse[]>([]); //List of all courses
  private allTopics = signal<ModelTopicsResponse[]>([]); //List of all topics
  //Consume the signal allUsers, and return all Users and Unauthorized's
  users = computed<AdminUsers[]>(() => {
    const list = this.allUsers() ?? [];
    return list.filter((u) => u.Role === RolesTypes.User || u.Role === RolesTypes.Unauthorized);
  });
  //Consume the signal allUsers, and return all Teachers
  teachers = computed<AdminUsers[]>(() => {
    return this.allUsers().filter(
      (u) => u.Role === RolesTypes.Teacher || u.Role === RolesTypes.Admin
    );
  });

  usersQuery = signal<string>(''); //Search query Name + Username + Role
  teachersQuery = signal<string>(''); //Search query Name + Username + Status
  topicsQuery = signal<string>(''); //Search query Type + Description
  courseQuery = signal<string>(''); //Search Name + Description + Status + Date

  courseStatusFilter = signal<string>(''); //All status of courses
  typesFilter = signal<string>(''); //All types of courses
  courseDateFrom = signal<string>(''); // yyyy-MM-dd
  courseDateTo = signal<string>(''); // yyyy-MM-dd

  //Variable to create new topic
  newTopic = {
    Type: '',
    Description: '',
  };

  constructor(
    private userService: UserService,
    private eventService: EventService,
    private topicService: TopicsService,
    private roleService: RolesService
  ) {}

  ngOnInit(): void {
    this.getUsers();
    this.getCourses();
    this.getTopics();
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
          const mapped = res.map((x: any) => ({
            Id: x.Id ?? x.id,
            Name: x.Name ?? x.name,
            Description: x.Description ?? x.description,
            TopicName: x.TopicName ?? x.topicName,
            CreateBy: x.CreateBy ?? x.createBy,
            Status: x.Status ?? x.status,
            DateCreate: x.DateCreate ?? x.dateCreate,
            DateClose: x.DateClose ?? x.dateClose,
          }));

          mapped.sort((a, b) => {
            const da = new Date(a.DateCreate).getTime();
            const db = new Date(b.DateCreate).getTime();
            return db - da;
          });

          this.allCourses.set(mapped);
        }
      },
    });
  }
  //Get the all topics from backend
  private getTopics() {
    this.topicService.getTopics().subscribe({
      next: (res) => {
        if (Array.isArray(res) && res) {
          this.allTopics.set(
            res.map((x: any) => ({
              Id: x.Id ?? x.id,
              Type: x.Type ?? x.type,
              Description: x.Description ?? x.description,
            }))
          );
        } else {
          this.allTopics.set([]);
        }
      },
    });
  }

  //Function to filtered all users with or without filters
  filteredUsers = computed<AdminUsers[]>(() => {
    const list = this.users() ?? [];
    const q = this.normalize(this.usersQuery().trim());

    return list.filter((c) => {
      if (q) {
        const name = this.normalize(c.Name ?? '');
        const username = this.normalize(c.Username ?? '');
        const email = this.normalize(c.Email ?? '');
        const role = this.normalize(c.Role ?? '');
        if (!name.includes(q) && !username.includes(q) && !email.includes(q) && !role.includes(q))
          return false;
      }

      return true;
    });
  });
  //Function to filtered all teachers with or without filters
  filteredTeachers = computed<AdminUsers[]>(() => {
    const list = this.teachers() ?? [];
    const q = this.normalize(this.teachersQuery().trim());

    return list.filter((c) => {
      if (q) {
        const name = this.normalize(c.Name ?? '');
        const username = this.normalize(c.Username ?? '');
        const email = this.normalize(c.Email ?? '');
        if (!name.includes(q) && !username.includes(q) && !email.includes(q)) return false;
      }

      return true;
    });
  });
  //Function to filtered all courses with or without filters
  filteredCourses = computed<AdminCourse[]>(() => {
    const list = this.allCourses() ?? [];
    const q = this.normalize(this.courseQuery().trim());
    const status = this.courseStatusFilter();
    const types = this.typesFilter();
    const from = this.courseDateFrom();
    const to = this.courseDateTo();

    return list.filter((c) => {
      if (q) {
        const name = this.normalize(c.Name ?? '');
        const description = this.normalize(c.Description ?? '');
        if (!name.includes(q) && !description.includes(q)) return false;
      }

      if (types && c.TopicName !== types) return false;

      if (status && c.Status !== status) return false;

      const courseDate = new Date(c.DateCreate);

      if (from) {
        const dFrom = new Date(from);
        if (courseDate < dFrom) return false;
      }

      if (to) {
        const dTo = new Date(to);
        dTo.setHours(23, 59, 59, 999);
        if (courseDate > dTo) return false;
      }

      return true;
    });
  });
  //Function to filtered all topics with or without filters
  filteredTopics = computed<AdminTopics[]>(() => {
    const list = this.allTopics() ?? [];
    const q = this.normalize(this.topicsQuery().trim());

    return list.filter((c) => {
      if (q) {
        const type = this.normalize(c.Type ?? '');
        const description = this.normalize(c.Description ?? '');
        if (!type.includes(q) && !description.includes(q)) return false;
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
  //List of all types of the list course
  courseTypesOptions = computed<string[]>(() => {
    const list = this.allCourses() ?? [];
    const set = new Set<string>();

    list.forEach((c) => {
      if (c.TopicName) {
        set.add(c.TopicName);
      }
    });

    return Array.from(set).sort(); // opcional: ordena alfabeticamente
  });

  //Function to normalize strings
  private normalize(str: string): string {
    return str
      .normalize('NFD')
      .replace(/\p{Diacritic}/gu, '')
      .toLowerCase();
  }
  //Function to change role of specific user
  onRoleChange(user: AdminUsers, newRole: string) {
    const role = newRole as RolesTypes;

    const obj = {
      Id: user.Id,
      Email: user.Email,
      Type: role,
    };

    if (!obj.Id || !obj.Email || !obj.Type) {
      return;
    }

    this.roleService.updateParticipantStatus(obj).subscribe({
      next: (res) => {
        console.log(res);
        if (!res) {
          alert('User role change successful.');
          return;
        }
        //? Check this
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
      },
    });
  }
  //Button to reset all filters of courses
  btnResetCourseFilters() {
    this.courseQuery.set('');
    this.courseStatusFilter.set('');
    this.courseDateFrom.set('');
    this.courseDateTo.set('');
  }
  //Button to delete a specific topic
  btnDeleteTopic(topic: { Id: number; Type: string; Description: string }) {
    if (!confirm(`Are you sure you want to delete the topic "${topic.Type}"?`)) {
      return;
    }

    this.topicService.deleteTopics(topic).subscribe({
      next: (res) => {
        if (!res) {
          alert('Topic impossible to delete.');
        }
        this.allTopics.update((list) => list.filter((t) => t.Id !== topic.Id));
        alert('Topic deleted successful.');
        return;
      },
    });
  }
  //Function to create new topics
  createTopic() {
    if (!this.newTopic.Description || !this.newTopic.Type) {
      alert('Fill the fields.');
      return;
    }

    this.topicService.insertTopics(this.newTopic).subscribe({
      next: (res) => {
        if (!res) {
          alert('Topic not created.');
          return;
        }
        alert('Topic created successfully.');
        this.getTopics();
        this.newTopic = { Type: '', Description: '' };
      },
    });
  }
  //TrackBy to help and performance in list courses
  trackByTopicId = (_: number, topic: any) => topic.Id;
  trackByUserId: TrackByFunction<AdminUsers> = (_, item) => item.Id;
  trackByCourseId: TrackByFunction<AdminCourse> = (_, item) => item.Id;
}
