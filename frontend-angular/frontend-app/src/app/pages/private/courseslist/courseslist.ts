import { CommonModule } from '@angular/common';
import { Component, computed, signal, TrackByFunction } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../../services/events';
import { EventListResponse, ModelEventsRequest } from '../../../models/modelEvents';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-courseslist',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './courseslist.html',
  styleUrl: './courseslist.scss',
})
export class CoursesList {
  //List of courses
  private allCourses = signal<EventListResponse[]>([]);

  //Filters query
  query = signal<string>(''); //Filter name + description
  typeFilter = signal<string>(''); //Filter Topic course
  //Filter date
  dateFrom = signal<string>('');
  dateTo = signal<string>('');

  //Loading view
  loading = signal<boolean>(false);

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadCourses();
  }
  //Function to get all courses from backend
  private loadCourses() {
    this.loading.set(true);
    this.eventService.getEvents().subscribe({
      next: (res) => {
        if (Array.isArray(res)) {
          this.loading.set(false);

          const mapped = res.map((e) => ({
            Id: e.Id || e.id,
            Name: e.Name || e.name,
            Description: e.Description || e.description,
            TopicName: e.TopicName || e.topicName,
            CreateById: e.CreateById || e.createById,
            DateCreate: e.DateCreate || e.dateCreate,
            DateClose: e.DateClose || e.dateClose,
            Status: e.Status || e.status,
          }));

          const activeMapped = mapped.filter((e) => e.Status === "Open"); //Filtered courses by status open

          this.allCourses.set(activeMapped);
          return;
        }
        this.loading.set(false);
        this.allCourses.set([]);
        return;
      },
    });
  }
  //List with all courses type
  courseTypes = computed<string[]>(() => {
    const set = new Set<string>();
    this.allCourses().forEach((c) => {
      if (c.TopicName) set.add(c.TopicName);
    });
    return Array.from(set).sort();
  });

  //Function to normalize the string (lower case + without accentuation)
  private normalize(str: string): string {
    return str
      .normalize('NFD')
      .replace(/\p{Diacritic}/gu, '')
      .toLowerCase();
  }
  //Courses filtered with base all filters
  filteredCourses = computed<EventListResponse[]>(() => {
    const list = this.allCourses() ?? [];
    const q = this.normalize(this.query().trim());
    const type = this.typeFilter();
    const from = this.dateFrom();
    const to = this.dateTo();

    return list.filter((c) => {
      //Filter by (Name + Description + TopicName + DateCreate)
      if (q) {
        const name = this.normalize(c.Name ?? '');
        const desc = this.normalize(c.Description ?? '');
        const topic = this.normalize(c.TopicName ?? '');

        //DateCreate always come with string ISO: "2025-10-22T15:26:29.700Z"
        const rawDate = c.DateCreate ?? '';
        const dateOnly = rawDate.slice(0, 10); // "2025-10-22"
        const normalizedDate = this.normalize(dateOnly);

        const matchesText =
          name.includes(q) || desc.includes(q) || topic.includes(q) || normalizedDate.includes(q);

        if (!matchesText) return false;
      }

      //Filter by topic course
      if (type && c.TopicName !== type) return false;

      //Filter by date (from / to)
      if (from) {
        const dFrom = new Date(from);
        const dCourse = new Date(c.DateCreate);
        if (dCourse < dFrom) return false;
      }

      if (to) {
        const dTo = new Date(to);
        const dCourse = new Date(c.DateCreate);
        dTo.setHours(23, 59, 59, 999); //Include one day integral
        if (dCourse > dTo) return false;
      }

      return true;
    });
  });
  //Button to reset all filters insert
  resetFilters() {
    this.query.set('');
    this.typeFilter.set('');
    this.dateFrom.set('');
    this.dateTo.set('');
  }
  //TrackBy to help and performance in list courses
  trackById: TrackByFunction<EventListResponse> = (_: number, item) => item.Id;
}
