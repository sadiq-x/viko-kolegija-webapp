import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-admin-events',
  imports: [],
  templateUrl: './admin-events.html',
  styleUrl: './admin-events.scss'
})
export class AdminEvents {
  private eventId: number = 0;
  private coursePassed: any;


  constructor(private route: ActivatedRoute){}

  ngOnInit(): void {
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.coursePassed = history.state?.['course'];

    if (!this.coursePassed && this.coursePassed.Id != this.eventId) {
      return;
    }

    console.log(this.coursePassed)
    console.log(this.eventId)
  }
}
