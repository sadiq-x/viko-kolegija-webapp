import { CommonModule } from '@angular/common';
import { Component, computed, EventEmitter, signal, TrackByFunction } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { ModelTopicsResponse } from '../../../models/modelTopics';
import { TopicsService } from '../../../services/topics';
var f;
@Component({
  selector: 'app-topics',
  imports: [CommonModule, FormsModule],
  templateUrl: './topics.html',
  styleUrls: ['./topics.scss'],
})
export class Topics {
  /** Lista que vem do backend */
  topics: ModelTopicsResponse[] = [];

  trackByIndex = (index: number) => index;

  /** Estado de carregamento */
  loading = false;

  /** (opcional) Evento quando o utilizador escolhe um tópico */
  selectTopic = new EventEmitter<ModelTopicsResponse>();
  
  //Search variable - signal
  query = signal<string>('');

  /** Skeletons para loading */
  loadingSkeletons = Array.from({ length: 6 });
  // TODO: Rever o codigo topo do topics
  //Filtered List (Type + Description) 
  filtered = computed<ModelTopicsResponse[]>(() => {
    const q = this.query().trim().toLowerCase();
    if (!q) return this.topics ?? [];
    return (this.topics ?? []).filter(
      (t) =>
        (t.Type ?? '').toLowerCase().includes(q) || (t.Description ?? '').toLowerCase().includes(q)
    );
  });
  
  f = computed(()=>{});

  constructor(private topicService: TopicsService) {}

  ngOnInit() {
    this.loadTopics();
  };

  loadTopics() {
    this.loading = true; //Turn on before the request
    this.topicService.getTopics().subscribe({
      next: (res: ModelTopicsResponse[] | false) => {
        //Verify if the res are false or not
        if (res == false || !Array.isArray(res)){
          this.topics = []; //Set the loading false
          this.loading = false;
          return;
        }

        this.loading = false; //Set the loading false
        const list = Array.isArray(res) ? res : [];
        //Mapped for the interface with PascalCase
        this.topics = list.map((x: any) => ({
          Id: x.Id ?? x.id,
          Type: x.Type ?? x.type,
          Description: x.Description ?? x.description,
        }));
        // (debug)
        // console.log('topics:', this.topics);
      },
      error: () => {
        this.topics = [];
        this.loading = false;
      },
    });
  }

  trackById: TrackByFunction<ModelTopicsResponse> = (_: number, item) => item.Id;

  onSelect(t: ModelTopicsResponse) {
    console.log('Selected topic:', t);
    this.selectTopic.emit(t);
  }
}
