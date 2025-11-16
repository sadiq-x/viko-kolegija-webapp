import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EventService } from '../../../services/events';
import { EventListResponse } from '../../../models/modelEvents';

@Component({
  selector: 'app-coursestype',
  imports: [CommonModule, RouterLink],
  templateUrl: './coursestype.html',
  styleUrl: './coursestype.scss',
})
export class CoursesType {
  type: string = '';
  loading: boolean = true;

  courses = signal<EventListResponse[]>([]);

  constructor(private route: ActivatedRoute, private eventService: EventService) {}

  ngOnInit(): void {
    this.getCourseFromTopicsPage();
  }
  //Function to get event by Topic selected
  getCourseFromTopicsPage() {
    const passed = history.state?.['course']; //Get the type from state received from topics page
    this.type = (this.route.snapshot.paramMap.get('type') ?? '').trim(); //Get the Type from url params

    if (passed.Type === this.type) {
      this.loading = true;
      const obj = {
        Topic: this.type || passed.Type,
      };

      this.eventService.getEventByTopic(obj).subscribe({
        next: (res) => {
          if (Array.isArray(res)) {
            const mapped = res.map((p: any) => ({
              Id: p.Id ?? p.id,
              Name: p.Name ?? p.name,
              Description: p.Description ?? p.description,
              TopicName: p.TopicName ?? p.topicName,
              CreateBy: p.CreateBy ?? p.createBy,
              DateCreate: p.DateCreate ?? p.dateCreate,
              DateClose: p.DateClose ?? p.dateClose,
              Status: p.Status ?? p.status,
            }));
            this.courses.set(mapped.filter((e) => e.Status === "Open"));
            this.loading = false;
          } else {
            this.courses.set([]);
            this.loading = false;
          }
        },
      });
      return;
    } else {
      this.courses.set([]);
      this.loading = false;
    }
  }
  //
}
